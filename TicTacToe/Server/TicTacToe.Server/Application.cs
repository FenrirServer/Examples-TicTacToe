using Fenrir.Multiplayer.Server;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace TicTacToe.Server
{
    class Application
    {
        private readonly ILogger<Application> _logger;
        private readonly NetworkServer _networkServer;

        TaskCompletionSource<int> _runTcs = new TaskCompletionSource<int>();

        public Application(ILogger<Application> logger, NetworkServer networkServer)
        {
            _logger = logger;
            _networkServer = networkServer;
        }

        public Task<int> Run()
        {
            _networkServer.Start();
            return _runTcs.Task;
        }

        public Task Shutdown(int exitCode)
        {
            _logger.LogInformation("Shutting down");
            _runTcs.SetResult(exitCode);

            // Graceful shutdown: wait for all players to disconnect
            return Task.CompletedTask; 
        }
    }
}
