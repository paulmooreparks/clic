using Microsoft.Extensions.DependencyInjection;
using Cliffer;

namespace Clic;

internal class ClicProgram {
    static async Task<int> Main(string[] args) {
        var cli = new ClifferBuilder()
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

