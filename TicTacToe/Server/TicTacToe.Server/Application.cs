using Fenrir.Multiplayer;
using Fenrir.Multiplayer.Rooms;
using System.Threading.Tasks;
using TicTacToe.Shared;

namespace TicTacToe.Server
{
    class Application : IRequestHandlerAsync<TicTacToeMoveRequest, TicTacToeMoveResponse>
    {
        private readonly FenrirLogger _logger;
        private readonly NetworkServer _networkServer;

        TaskCompletionSource<int> _runTcs = new TaskCompletionSource<int>();

        public Application(FenrirLogger fenrirLogger, NetworkServer networkServer)
        {
            _logger = fenrirLogger;
            _networkServer = networkServer;
        }

        public Task<int> Run()
        {
            // Add TicTacToe room management
            _networkServer.AddRooms(CreateNewTicTacToeRoom);

            // Add request handlers
            _networkServer.AddRequestHandlerAsync<TicTacToeMoveRequest, TicTacToeMoveResponse>(this);

            // Start server
            _networkServer.Start();

            _logger.Info("Started server application");

            return _runTcs.Task;
        }

        private TicTacToeServerRoom CreateNewTicTacToeRoom(IServerPeer peer, string roomId, string joinToken)
        {
            // Do not check join token or room id
            return new TicTacToeServerRoom(_logger, roomId);
        }

        public Task Shutdown(int exitCode)
        {
            _logger.Info("Shutting down");
            _runTcs.SetResult(exitCode);

            // Graceful shutdown: wait for all players to disconnect
            return Task.CompletedTask; 
        }

        #region Request Handlers
        async Task<TicTacToeMoveResponse> IRequestHandlerAsync<TicTacToeMoveRequest, TicTacToeMoveResponse>.HandleRequestAsync(TicTacToeMoveRequest request, IServerPeer peer)
        {
            TaskCompletionSource<TicTacToeMoveResponse> tcs = new TaskCompletionSource<TicTacToeMoveResponse>();

            // Get player
            if (peer.PeerData == null)
            {
                // Not in the room
                return new TicTacToeMoveResponse() { Success = false };
            }
            var player = (TicTacToeServerPlayer)peer.PeerData;

            // Get room
            var room = player.Room;

            // Dispatch message to the room
            TicTacToeMoveResponse response = await room.ExecuteAsync<TicTacToeMoveResponse>(() => room.DispatchMoveRequest(request, player));
            return response;
        }
        #endregion
    }
}
