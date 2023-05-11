using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emby.Naming.Common;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Tasks;

namespace Jellyfin.Plugin.FolderPlaylist
{
    /// <inheritdoc/>
    public class GeneratePlaylists : IScheduledTask
    {
        private readonly ILibraryManager _libraryManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratePlaylists"/> class.
        /// </summary>
        /// <param name="libraryManager">Instance of the <see cref="ILibraryManager"/> interface.</param>
        public GeneratePlaylists(ILibraryManager libraryManager)
        {
            _libraryManager = libraryManager;
        }

        /// <inheritdoc/>
        public string Name => "Generate Folder Playlists";

        /// <inheritdoc/>
        public string Key => "Generate Folder Playlists";

        /// <inheritdoc/>
        public string Description => "Generate Folder Playlists";

        /// <inheritdoc/>
        public string Category => "Library";

        /// <inheritdoc/>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var allMusicLibs = _libraryManager.RootFolder.Children.Where(x => _libraryManager.GetContentType(x) == "music");
            var instance = Plugin.Instance!;
            var excludes = instance != null ? instance.Configuration.LibrariesSkip.Split(';') : Array.Empty<string>();
            // var excludesString = Plugin.Instance!.Configuration.LibrariesSkip;
            // var excludes = excludesString != null ? excludesString.Split(';') : Array.Empty<string>();
            var musicLibs = allMusicLibs.Where(x => !excludes.Contains(x.Name, StringComparer.OrdinalIgnoreCase));
            var namingOptions = new NamingOptions();
            foreach (var musicLib in musicLibs)
            {
                var topLevelFolder = musicLib.Path;
                var firstLevelFolders = Directory.GetDirectories(topLevelFolder, "*", SearchOption.TopDirectoryOnly);
                foreach (var folder in firstLevelFolders)
                {
                    var playlistFileName = Path.GetFileName(folder) + "-folder.m3u";
                    var playlistFile = Path.Combine(folder, playlistFileName);
                    var files = Directory.GetFiles(folder, "*", SearchOption.AllDirectories)
                        .Where(x => namingOptions.AudioFileExtensions.Contains(Path.GetExtension(x)))
                        .Select(y => Path.GetRelativePath(folder, y));
                    File.WriteAllLines(playlistFile, files, Encoding.UTF8);
                }
            }
        }

        /// <inheritdoc/>
        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
        {
            var trigger1 = new TaskTriggerInfo { Type = TaskTriggerInfo.TriggerStartup };
            var trigger2 = new TaskTriggerInfo()
            {
                Type = TaskTriggerInfo.TriggerInterval, IntervalTicks = TimeSpan.FromHours(6).Ticks
            };
            return new List<TaskTriggerInfo> { trigger1, trigger2 };
        }
    }
}
