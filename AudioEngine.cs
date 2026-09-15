using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using NAudio.Wave;

namespace BrushSound
{
    public class AudioChannel : IDisposable
    {
        private WaveOutEvent output;
        private AudioFileReader reader;
        private bool looping;      // 单文件循环
        private bool listLoop;     // 列表循环
        private bool stoppingManually;

        private volatile bool _manualPlaying = false;

        private int clipMs = 0;
        private DateTime clipStartTime;
        private int clipElapsedMs = 0;

        public DateTime LastUsedTime { get; private set; } = DateTime.UtcNow;
        public string CurrentPath { get; private set; }
        public string CurrentFolder { get; private set; }
        public int LastClipMs { get; private set; }
        public bool IsActive { get { return output != null; } }

        public event Action TrackEnded;

        public bool IsPaused
        {
            get
            {
                try { return output != null && output.PlaybackState == PlaybackState.Paused; }
                catch { return false; }
            }
        }

        public bool IsPlaying { get { return _manualPlaying; } }

        public bool Play(string path, string folderPath, bool loop, bool listLoopMode, int newClipMs)
        {
            LastUsedTime = DateTime.UtcNow;
            listLoop = listLoopMode;
            looping = loop && !listLoopMode;

            if (reader != null && output != null && CurrentPath == path)
            {
                try
                {
                    reader.Position = 0;
                    LastClipMs = newClipMs;
                    clipMs = newClipMs;
                    clipElapsedMs = 0;
                    clipStartTime = DateTime.UtcNow;
                    stoppingManually = false;
                    output.Play();
                    _manualPlaying = true;
                    return true;
                }
                catch { ReleaseResources(); }
            }

            ReleaseResources();
            try
            {
                reader = new AudioFileReader(path);
                output = new WaveOutEvent { DesiredLatency = 60 };
                output.Init(reader);
                output.PlaybackStopped += OnPlaybackStopped;
                stoppingManually = false;
                CurrentPath = path;
                CurrentFolder = folderPath;
                LastClipMs = newClipMs;
                clipMs = newClipMs;
                clipElapsedMs = 0;
                clipStartTime = DateTime.UtcNow;
                output.Play();
                _manualPlaying = true;
                return true;
            }
            catch
            {
                ReleaseResources();
                return false;
            }
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (stoppingManually) return;

            if (looping)
            {
                try
                {
                    if (reader != null && output != null)
                    {
                        reader.Position = 0;
                        clipStartTime = DateTime.UtcNow;
                        output.Play();
                    }
                }
                catch { }
                return;
            }

            if (listLoop)
            {
                try { TrackEnded?.Invoke(); } catch { }
                return;
            }

            _manualPlaying = false;
        }

        public void Pause()
        {
            LastUsedTime = DateTime.UtcNow;
            try { output?.Pause(); } catch { }
            _manualPlaying = false;
            if (clipMs > 0)
                clipElapsedMs += (int)(DateTime.UtcNow - clipStartTime).TotalMilliseconds;
        }

        public void Resume()
        {
            LastUsedTime = DateTime.UtcNow;
            try { output?.Play(); } catch { }
            _manualPlaying = true;
            if (clipMs > 0) clipStartTime = DateTime.UtcNow;
        }

        public void SetVolume(float vol)
        {
            try { if (reader != null) reader.Volume = vol; } catch { }
        }

        public void CheckClip()
        {
            if (clipMs <= 0) return;
            if (output == null) return;
            if (!_manualPlaying) return;

            PlaybackState state;
            try { state = output.PlaybackState; }
            catch { return; }
            if (state != PlaybackState.Playing) return;

            int elapsed = clipElapsedMs + (int)(DateTime.UtcNow - clipStartTime).TotalMilliseconds;
            if (elapsed < clipMs) return;

            if (looping || listLoop)
            {
                try
                {
                    reader.Position = 0;
                    clipStartTime = DateTime.UtcNow;
                    clipElapsedMs = 0;
                }
                catch { }
            }
            else
            {
                stoppingManually = true;
                _manualPlaying = false;
                try { output.Stop(); } catch { }
            }
        }

        public void Stop()
        {
            LastUsedTime = DateTime.UtcNow;
            stoppingManually = true;
            _manualPlaying = false;
            try { output?.Stop(); } catch { }
        }

        public void ReleaseResources()
        {
            stoppingManually = true;
            _manualPlaying = false;
            try { output?.Stop(); } catch { }
            try { output?.Dispose(); } catch { }
            try { reader?.Dispose(); } catch { }
            output = null;
            reader = null;
            CurrentPath = null;
            CurrentFolder = null;
            LastClipMs = 0;
            clipMs = 0;
            clipElapsedMs = 0;
        }

        public void Dispose() { ReleaseResources(); }
    }

    public class AudioManager : IDisposable
    {
        public string SoundFolder { get; set; }

        private int volume = 80;
        public int Volume
        {
            get { return volume; }
            set
            {
                volume = Math.Max(0, Math.Min(100, value));
                Enqueue(() =>
                {
                    foreach (var ch in channels.Values)
                        ch.SetVolume(volume / 100f);
                });
            }
        }

        public static readonly string[] SupportedExtensions = { ".wav", ".mp3", ".aiff", ".aif" };

        private readonly Dictionary<string, AudioChannel> channels = new Dictionary<string, AudioChannel>();
        private readonly HashSet<string> holdingCombos = new HashSet<string>();
        private readonly Dictionary<string, int> seqIndex = new Dictionary<string, int>();
        private readonly HashSet<string> failedFiles = new HashSet<string>();
        private readonly Dictionary<string, FolderCacheEntry> folderCache = new Dictionary<string, FolderCacheEntry>();

        /// <summary>正在播放事件： (combo, fileName)，fileName 为空表示停止</summary>
        public event Action<string, string> TrackChanged;

        private class FolderCacheEntry
        {
            public DateTime Time;
            public List<string> Files;
        }

        private readonly BlockingCollection<Action> taskQueue = new BlockingCollection<Action>();
        private readonly Thread workerThread;
        private volatile bool disposed = false;

        private const int ClipCheckIntervalMs = 15;
        private const int IdleTimeoutMs = 8000;
        private DateTime lastIdleCheck = DateTime.UtcNow;

        public AudioManager(string soundFolder)
        {
            SoundFolder = soundFolder;
            workerThread = new Thread(WorkerLoop)
            {
                IsBackground = true,
                Name = "AudioWorker",
                Priority = ThreadPriority.AboveNormal
            };
            workerThread.Start();
        }

        private void WorkerLoop()
        {
            while (!disposed)
            {
                Action task;
                bool has = false;
                try { has = taskQueue.TryTake(out task, ClipCheckIntervalMs); }
                catch { break; }

                if (has)
                {
                    try { task(); } catch { }
                }
                else
                {
                    try { foreach (var ch in channels.Values) ch.CheckClip(); } catch { }

                    if ((DateTime.UtcNow - lastIdleCheck).TotalMilliseconds > 2000)
                    {
                        lastIdleCheck = DateTime.UtcNow;
                        try { CleanupIdleChannels(); } catch { }
                    }
                }
            }
        }

        private void CleanupIdleChannels()
        {
            var now = DateTime.UtcNow;
            List<string> toRelease = null;

            foreach (var kv in channels)
            {
                var ch = kv.Value;
                if (!ch.IsActive) continue;
                if (ch.IsPlaying) continue;
                if (ch.IsPaused) continue;

                if ((now - ch.LastUsedTime).TotalMilliseconds > IdleTimeoutMs)
                {
                    if (toRelease == null) toRelease = new List<string>();
                    toRelease.Add(kv.Key);
                }
            }

            if (toRelease != null)
            {
                foreach (var key in toRelease)
                {
                    AudioChannel ch;
                    if (channels.TryGetValue(key, out ch)) ch.ReleaseResources();
                }
            }
        }

        private void Enqueue(Action action)
        {
            if (disposed) return;
            try { taskQueue.Add(action); } catch { }
        }

        public void Trigger(string combo, SoundMapping mapping)
        {
            if (mapping == null) return;
            Enqueue(() =>
            {
                holdingCombos.Add(combo);
                DoTrigger(combo, mapping);
            });
        }

        public void Release(string combo, bool resume)
        {
            Enqueue(() =>
            {
                holdingCombos.Remove(combo);
                DoRelease(combo, resume);
            });
        }

        public void Stop(string combo)
        {
            Enqueue(() => DoStop(combo));
        }

        public void StopAll()
        {
            Enqueue(DoStopAll);
        }

        public void ClearFailed()
        {
            Enqueue(() =>
            {
                failedFiles.Clear();
                folderCache.Clear();
            });
        }

        private void DoTrigger(string combo, SoundMapping mapping)
        {
            AudioChannel ch = GetOrCreateChannel(combo);

            if (mapping.Resume && ch.IsActive && ch.IsPaused && SourceMatches(ch, mapping))
            {
                ch.Resume();
                return;
            }

            if (ch.IsPlaying && SourceMatches(ch, mapping)) return;

            string path = ResolvePath(combo, mapping);
            if (path == null) return;

            bool listLoopMode = mapping.IsFolderMode && mapping.Loop;

            if (!ch.Play(path, GetFolderPath(mapping.Folder), mapping.Loop, listLoopMode, mapping.ClipMs))
            {
                failedFiles.Add(path);
                string nextPath = ResolvePath(combo, mapping);
                if (nextPath != null && nextPath != path)
                {
                    if (!ch.Play(nextPath, GetFolderPath(mapping.Folder), mapping.Loop, listLoopMode, mapping.ClipMs))
                        failedFiles.Add(nextPath);
                }
            }
            ch.SetVolume(volume / 100f);

            try { TrackChanged?.Invoke(combo, Path.GetFileName(path)); } catch { }
        }

        private void DoRelease(string combo, bool resume)
        {
            AudioChannel ch;
            if (!channels.TryGetValue(combo, out ch)) return;
            if (resume) ch.Pause();
            else ch.Stop();
        }

        private void DoStop(string combo)
        {
            AudioChannel ch;
            if (channels.TryGetValue(combo, out ch)) ch.Stop();
        }

        private void DoStopAll()
        {
            foreach (var ch in channels.Values) ch.Stop();
        }

        private AudioChannel GetOrCreateChannel(string combo)
        {
            AudioChannel ch;
            if (!channels.TryGetValue(combo, out ch))
            {
                ch = new AudioChannel();
                string capturedCombo = combo;
                ch.TrackEnded += () => Enqueue(() => HandleTrackEnded(capturedCombo));
                channels[combo] = ch;
            }
            return ch;
        }

        private void HandleTrackEnded(string combo)
        {
            if (!holdingCombos.Contains(combo)) return;

            AudioChannel ch;
            if (!channels.TryGetValue(combo, out ch)) return;
            if (!ch.IsActive) return;

            SoundMapping mapping;
            if (!ConfigManager.Current.Mappings.TryGetValue(combo, out mapping)) return;
            if (!mapping.IsFolderMode || !mapping.Loop) return;

            string path = ResolvePath(combo, mapping);
            if (path == null) return;

            if (!ch.Play(path, GetFolderPath(mapping.Folder), mapping.Loop, true, mapping.ClipMs))
                failedFiles.Add(path);
            ch.SetVolume(volume / 100f);

            try { TrackChanged?.Invoke(combo, Path.GetFileName(path)); } catch { }
        }

        private string GetFolderPath(string folderName)
        {
            return string.IsNullOrEmpty(folderName)
                ? SoundFolder
                : Path.Combine(SoundFolder, folderName);
        }

        private bool SourceMatches(AudioChannel ch, SoundMapping mapping)
        {
            if (ch.CurrentPath == null) return false;
            if (ch.LastClipMs != mapping.ClipMs) return false;

            string folderPath = GetFolderPath(mapping.Folder);

            if (!mapping.IsFolderMode)
            {
                if (string.IsNullOrEmpty(mapping.File)) return false;
                string expected = Path.Combine(folderPath, mapping.File);
                return string.Equals(Path.GetFullPath(ch.CurrentPath),
                                     Path.GetFullPath(expected),
                                     StringComparison.OrdinalIgnoreCase);
            }

            try
            {
                string currentDir = Path.GetDirectoryName(ch.CurrentPath);
                return string.Equals(Path.GetFullPath(currentDir),
                                     Path.GetFullPath(folderPath),
                                     StringComparison.OrdinalIgnoreCase);
            }
            catch { return false; }
        }

        private List<string> GetFolderFiles(string folderPath)
        {
            try
            {
                if (!Directory.Exists(folderPath)) return null;

                DateTime mtime = Directory.GetLastWriteTime(folderPath);
                FolderCacheEntry cached;
                if (folderCache.TryGetValue(folderPath, out cached))
                {
                    if (cached.Time == mtime && cached.Files != null) return cached.Files;
                }

                var files = new List<string>();
                foreach (var f in Directory.GetFiles(folderPath))
                {
                    string ext = Path.GetExtension(f).ToLower();
                    if (Array.IndexOf(SupportedExtensions, ext) >= 0) files.Add(f);
                }
                files.Sort(StringComparer.OrdinalIgnoreCase);

                folderCache[folderPath] = new FolderCacheEntry { Time = mtime, Files = files };
                return files;
            }
            catch { return null; }
        }

        private string ResolvePath(string combo, SoundMapping mapping)
        {
            if (mapping == null) return null;
            if (string.IsNullOrEmpty(mapping.Folder)) return null;

            string folderPath = GetFolderPath(mapping.Folder);

            if (!mapping.IsFolderMode)
            {
                if (string.IsNullOrEmpty(mapping.File)) return null;
                string path = Path.Combine(folderPath, mapping.File);
                if (!File.Exists(path) || failedFiles.Contains(path)) return null;
                return path;
            }

            var allFiles = GetFolderFiles(folderPath);
            if (allFiles == null || allFiles.Count == 0) return null;

            var files = new List<string>();
            foreach (var f in allFiles)
                if (!failedFiles.Contains(f)) files.Add(f);

            if (files.Count == 0) return null;

            if (mapping.Random)
            {
                var rand = new Random();
                return files[rand.Next(files.Count)];
            }

            int idx = seqIndex.ContainsKey(combo) ? seqIndex[combo] : 0;
            string chosen = files[idx % files.Count];
            seqIndex[combo] = (idx + 1) % files.Count;
            return chosen;
        }

        public void Dispose()
        {
            disposed = true;
            try { taskQueue.CompleteAdding(); } catch { }
            try { workerThread.Join(500); } catch { }

            foreach (var ch in channels.Values)
            {
                try { ch.Dispose(); } catch { }
            }
            channels.Clear();
        }
    }
}