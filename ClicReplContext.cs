using System.Reflection;

namespace Clic;

internal class ClicReplContext : Cliffer.DefaultReplContext {
    public string Title => "clic, the CLI Stack Calculator";
    public override string[] GetPopCommands() => [];

    public override string GetTitleMessage() {
        Assembly assembly = Assembly.GetExecutingAssembly();
        Version? version = assembly.GetName().Version;
        string versionString = version?.ToString() ?? "Unknown";
        return $"{Title} v{versionString}";
    }
}

