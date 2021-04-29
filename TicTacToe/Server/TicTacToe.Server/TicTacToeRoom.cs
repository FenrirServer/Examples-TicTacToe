using Fenrir.Multiplayer.Logging;
using Fenrir.Multiplayer.Network;
using Fenrir.Multiplayer.Rooms;
using System;
using System.Linq;
using System.Threading.Tasks;
using TicTacToe.Shared;

namespace TicTacToe.Server
{
    class TicTacToeRoom : ServerRoom
    {
        // Seconds to make a move
        private const float SecondsPerMove = 5;

        /// <summary>
        /// State of the game
        /// </summary>
        private GameState _gameState = GameState.WaitingForPlayers;

        /// <summary>
        /// Game board
        /// </summary>
        private char[] _board = new char[9];

        /// <summary>
        /// Random numbers generator
        /// </summary>
        private Random _rng = new Random();

        /// <summary>
        /// Current move number
        /// </summary>
        private int _currentMoveNumber = 0;


        /// <summary>
        /// Current player
        /// </summary>
        private TicTacToePlayer _currentPlayer = null;

        public TicTacToeRoom(ILogger logger, string roomId)
            : base(logger, roomId)
        {
            // Constructor...
        }


        protected override RoomJoinResponse OnBeforePeerJoin(IServerPeer peer, string token)
        {
            // This method allows to validate peer before they join

            if(peer.PeerData != null)
            {
                return new RoomJoinResponse(false, 1, "Already in a room");
            }

            if(_gameState != GameState.WaitingForPlayers)
            {
                return new RoomJoinResponse(false, 1, "Game already started");
            }

            return RoomJoinResponse.JoinSuccess;
        }

        protected override void OnPeerJoin(IServerPeer peer, string token)
        {
            // This method is called when new peer joins this room

            string playerName = token; // Use join token as a name ;)

            var player = new TicTacToePlayer(peer, this, playerName);
            peer.PeerData = player;

            // If both players join, start game
            if(Peers.Count > 1)
            {
                StartGame();
            }
        }

        protected override void OnPeerLeave(IServerPeer peer)
        {
            // This method is called peer leaves the room, or disconnects

            var player = (TicTacToePlayer)peer.PeerData;

            // Force finish the game
            if(_gameState == GameState.Started)
            {
                var otherPeer = Peers.Values.Where(p => p != peer).First();
                var otherPlayer = (TicTacToePlayer)otherPeer.PeerData;
                FinishGame(otherPlayer);
            }
        }

        private TicTacToePlayer[] GetPlayers()
        {
            TicTacToePlayer[] players = Peers.Values.Select(peer => peer.PeerData)
                .Cast<TicTacToePlayer>()
                .ToArray();

            return players;
        }

        private void StartGame()
        {
            var players = GetPlayers();
            var player1 = players[0];
            var player2 = players[1];

            // Assign 'x' and 'o'
            player1.PieceType = _rng.Next(2) == 0 ? 'x' : 'o';
            player2.PieceType = player1.PieceType == 'x' ? 'o' : 'x';

            // Change game start
            _gameState = GameState.Started;

            // Set player - 'x' moves first
            _currentPlayer = player1.PieceType == 'x' ? player1 : player2;

            // Send out game started event
            var player1Reference = new TicTacToePlayerReference() { PeerId = player1.Peer.Id, Name = player1.Name, IsX = player1.IsX };
            var player2Reference = new TicTacToePlayerReference() { PeerId = player2.Peer.Id, Name = player2.Name, IsX = player2.IsX };
            var gameStartedEvent = new TicTacToeGameStartEvent() { Player1 = player1Reference, Player2 = player2Reference, SecondsPerMove = SecondsPerMove };
            BroadcastEvent<TicTacToeGameStartEvent>(gameStartedEvent);

            // Schedule move timeout
            ScheduleMoveTimeout();
        }

        private void ScheduleMoveTimeout()
        {
            int currentMove = _currentMoveNumber;
            Schedule(() => TimeoutMove(currentMove), TimeSpan.FromSeconds(SecondsPerMove));
        }

        private void TimeoutMove(int moveNumber)
        {
            if (_currentMoveNumber != moveNumber)
            {
                return; // Move already ended (player made a move before timer expired)
            }

            EndMove(true, 0);
        }

        private void EndMove(bool isTimeout, byte position)
        {
            var players = GetPlayers();

            // Broadcast move end event
            var moveEndEvent = new TicTacToeMoveEvent() { MoveNumber = _currentMoveNumber, IsTimeout = isTimeout, PieceType = _currentPlayer.PieceType, Position = position };
            BroadcastEvent<TicTacToeMoveEvent>(moveEndEvent);

            // Check if game has ended


            _currentMoveNumber++; // Next move
            _currentPlayer = _currentPlayer == players[0] ? players[1] : players[0]; // Swap current player

            ScheduleMoveTimeout();
        }

        private bool MakeMove(TicTacToeMoveRequest moveRequest, TicTacToePlayer player)
        {
            if(_board[moveRequest.Position] != default(char))
            {
                return false; // Move already made
            }
        }

        private bool IsWinningMove(int position, char pieceType)
        {
            byte len = 3;
            byte x = (byte)(position % len);
            byte y = (byte)(position / len);

            // Check horizontally
            for (int i = 0; i < len; i++)
            {
                if (_board[y * len + i] != pieceType)
                    break;
                if (i == len - 1)
                    return true;
            }

            // Check vertically
            for (int i = 0; i < len; i++)
            {
                if (_board[x * len + i] != pieceType)
                    break;
                if (i == len - 1)
                    return true;
            }

            // Check bottom-left to top-right diagonal
            if (x == y)
            {
                for (int i = 0; i < len; i++)
                {
                    if (_board[i * len + i] != pieceType)
                        break;
                    if (i == len - 1)
                        return true;
                }
            }

            // Check bottom-right to top-left diagonal
            if (x + y == len - 1)
            {
                for (int i = 0; i < len; i++)
                {
                    if (_board[i * len + (len - 1) - i] != pieceType)
                        break;
                    if (i == len - 1)
                        return true;
                }
            }

            return false;
        }


        private void FinishGame(TicTacToePlayer winner)
        {
            _gameState = GameState.Finished;

            var winnerReference = new TicTacToePlayerReference() { PeerId = winner.Peer.Id, Name = winner.Name, IsX = winner.IsX };
            var finishEvent = new TicTacToeGameFinishEvent() { Winner = winnerReference };
            BroadcastEvent<TicTacToeGameFinishEvent>(finishEvent);
        }


        public Task<TicTacToeMoveResponse> DispatchMoveRequestAsync(TicTacToeMoveRequest moveRequest, TicTacToePlayer player)
        {
            TaskCompletionSource<TicTacToeMoveResponse> tcs = new TaskCompletionSource<TicTacToeMoveResponse>();

            Enqueue(() =>
            {
                bool success = MakeMove(moveRequest, player);
                tcs.SetResult(new TicTacToeMoveResponse() { Success = success });
            });

            return tcs.Task;
        }

        enum GameState
        {
            WaitingForPlayers,
            Started,
            Finished,
        }
    }
}