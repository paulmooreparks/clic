using Cliffer;

namespace Clic;

internal static class EnumerableExtensions {
    static Dictionary<string, double> _variables;

    static EnumerableExtensions() {
        _variables = Utility.GetService<Dictionary<string, double>>()!;
    }

    internal static bool ProcessArgs(this IEnumerable<string> args, Action<double> action, Action<string> error) {
        foreach (var arg in args) {
            if (Constants.TryGetConstant(arg, out var constant)) {
                action(constant);
            }
            else if (_variables.TryGetValue(arg, out var variable)) {
                action(variable);
            }
            else {
                if (double.TryParse(arg, out double value)) {
                    action(value);
                }
                else {
                    Console.Error.WriteLine($"Error: Invalid number or constant '{arg}'");
                    return false;
                }
            }
        }

        return true;
    }
}
