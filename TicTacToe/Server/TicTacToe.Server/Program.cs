using Fenrir.Multiplayer.Rooms;
using Fenrir.Multiplayer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace TicTacToe.Server
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            // Dependency injection
            var serviceProvider = new ServiceCollection()
                .AddLogging(logging => logging.AddConsole())
                .AddSingleton<Fenrir.Multiplayer.ILogger, FenrirLogger>()
                .AddSingleton<FenrirLogger>()
                .AddSingleton<NetworkServer>()
                .AddSingleton<Application>()
                .BuildServiceProvider();

            // Get logger
            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();

            // Get application
            var application = serviceProvider.GetService<Application>();

            // Listen for SIGTERM
            AppDomain.CurrentDomain.ProcessExit += (_, _) => application.Shutdown(0).Wait();

            // Run until shut down
            return await application.Run();
        }
    }
}