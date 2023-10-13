using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.HeaderAuth.Configuration;

public class PluginConfiguration : BasePluginConfiguration
{
    public PluginConfiguration()
    {
        LoginHeader = "Tailscale-User-Login"
        NameHeader = "Tailscale-User-Name"
    }

    public string LoginHeader { set; get; }

    public string NameHeader { set; get; }
}
