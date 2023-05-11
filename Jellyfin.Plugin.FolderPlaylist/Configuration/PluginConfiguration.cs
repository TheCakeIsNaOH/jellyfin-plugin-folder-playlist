using Jellyfin.Data.Entities.Libraries;
using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.FolderPlaylist.Configuration;

/// <summary>
/// Plugin configuration.
/// </summary>
public class PluginConfiguration : BasePluginConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PluginConfiguration"/> class.
    /// </summary>
    public PluginConfiguration()
    {
        // set default options here
        Enabled = true;
        LibrariesSkip = string.Empty;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the playlist updates are enabled.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets a string setting.
    /// </summary>
    public string LibrariesSkip { get; set; }
}
