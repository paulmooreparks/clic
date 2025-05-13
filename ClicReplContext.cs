using System.CommandLine;
using System.Reflection;

namespace Clic;

internal class ClicReplContext(Command currentCommand) : Cliffer.DefaultReplContext(currentCommand) {
    public string Title => "clic, the CLI Stack Calculator";
    public override string[] PopCommands => [];

    public override string TitleMessage {
        get {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Version? version = assembly.GetName().Version;
            string versionString = version?.ToString() ?? "Unknown";
            return $"{Title} v{versionString}";
        }
    }
}

