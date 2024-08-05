using System.Text.Json;

namespace Clic;

public class PersistenceService {
    private readonly string _clicDirectory;
    private readonly string _stackFileName = "stack.txt";
    private readonly string _stackFilePath;
    private readonly string _variablesFileName = "variables.txt";
    private readonly string _variablesFilePath;
    private readonly Mutex _mutex;
    private readonly string _mutexName = "Global\\ClicMutex"; // Global mutex name for cross-process synchronization

    public PersistenceService() {
        var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        _clicDirectory = Path.Combine(homeDirectory, ".clic");

        if (!Directory.Exists(_clicDirectory)) {
            Directory.CreateDirectory(_clicDirectory);
        }

        _stackFilePath = Path.Combine(_clicDirectory, _stackFileName);
        _variablesFilePath = Path.Combine(_clicDirectory, _variablesFileName);
        _mutex = new Mutex(false, _mutexName);
    }

    public Stack<double> LoadStack() {
        var stack = new Stack<double>();

        if (File.Exists(_stackFilePath)) {
            try {
                _mutex.WaitOne();
                var lines = File.ReadLines(_stackFilePath).Reverse();

                foreach (var line in lines) {
                    if (double.TryParse(line, out var value)) {
                        stack.Push(value);
                    }
                }
            }
            finally {
                _mutex.ReleaseMutex();
            }
        }

        return stack;
    }

    public void SaveStack(Stack<double> stack) {
        try {
            if (Path.Exists(_clicDirectory)) {
                _mutex.WaitOne();
                var lines = stack.Select(x => x.ToString());
                File.WriteAllLines(_stackFilePath, lines);
            }
        }
        finally {
            _mutex.ReleaseMutex();
        }
    }

    public Dictionary<string, double> LoadVariables() {
        Dictionary<string, double> variables = new();

        if (File.Exists(_variablesFilePath)) {
            try {
                _mutex.WaitOne();
                var lines = File.ReadAllLines(_variablesFilePath);

                foreach (var line in lines) {
                    var kvp = line.Split('=');

                    if (kvp.Length == 2 && !string.IsNullOrEmpty(kvp[0]) && !string.IsNullOrEmpty(kvp[1])) {
                        var key = kvp[0];
                        var value = kvp[1];
                        if (double.TryParse(value, out var result)) {
                            variables[key] = result;
                        }
                    }
                }
            }
            finally {
                _mutex.ReleaseMutex();
            }
        }

        return variables;
    }

    public void SaveVariables(Dictionary<string, double> variables) {
        try {
            if (Path.Exists(_clicDirectory)) {
                _mutex.WaitOne();
                var lines = variables.Select(kvp => $"{kvp.Key}={kvp.Value}");
                File.WriteAllLines(_variablesFilePath, lines);
            }
        }
        finally {
            _mutex.ReleaseMutex();
        }
    }
}
