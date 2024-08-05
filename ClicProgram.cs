using Microsoft.Extensions.DependencyInjection;
using Cliffer;
using Microsoft.Extensions.Configuration;

namespace Clic;

internal class ClicProgram {
    private const string _clicDirectoryName = ".clic";
    private const string _configFileName = "appsettings.json";
    private static readonly string _configFilePath;
    private static readonly string _clicDirectory;

    static ClicProgram() {
        var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        _clicDirectory = Path.Combine(homeDirectory, _clicDirectoryName);

        if (!Directory.Exists(_clicDirectory)) {
            Directory.CreateDirectory(_clicDirectory);
        }

        _configFilePath = Path.Combine(_clicDirectory, _configFileName);
    }

    static async Task<int> Main(string[] args) {
        var cli = new ClifferBuilder()
            .ConfigureAppConfiguration((configurationBuiler) => {
                configurationBuiler.AddJsonFile(_configFilePath, true);
            })
            .ConfigureServices(services => {
                services.AddSingleton<PersistenceService>();
                services.AddSingleton<Stack<double>>(provider => {
                    var persistenceService = provider.GetService<PersistenceService>()!;
                    return persistenceService.LoadStack();
                });
                services.AddSingleton<Dictionary<string, double>>(provider => {
                    var persistenceService = provider.GetService<PersistenceService>()!;
                    return persistenceService.LoadVariables();
                });
            })
            .Build();

        Utility.SetServiceProvider(cli.ServiceProvider);

        ClifferEventHandler.OnExit += () => {
            var persistenceService = Utility.GetService<PersistenceService>()!;
            var stack = Utility.GetService<Stack<double>>()!;
            persistenceService.SaveStack(stack);
            var variables = Utility.GetService<Dictionary<string,double>>()!;
            persistenceService.SaveVariables(variables);
        };

        return await cli.RunAsync(args);
    }
}

