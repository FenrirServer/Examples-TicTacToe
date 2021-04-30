using Fenrir.Multiplayer.Logging;
using Fenrir.Multiplayer.Network;
using Fenrir.Multiplayer.Rooms;
using System;
using System.Linq;
using TicTacToe.Shared;

namespace TicTacToe.Server
{
    class TicTacToeServerRoom : ServerRoom
    {
        /// <summary>
        /// Number of seconds per move
        /// </summary>
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
        private TicTacToeServerPlayer _currentPlayer = null;


        public TicTacToeServerRoom(ILogger logger, string roomId)
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

            var player = new TicTacToeServerPlayer(peer, this, playerName);
            peer.PeerData = player;

            // If both players join, start the game
            if(Peers.Count > 1)
            {
                StartGame();
            }
        }

        protected override void OnPeerLeave(IServerPeer peer)
        {
            // This method is called peer leaves the room, or disconnects

            var player = (TicTacToeServerPlayer)peer.PeerData;

            // One player left during the game, force finish
            if(_gameState == GameState.Started)
            {
                var otherPeer = Peers.Values.Where(p => p != peer).First();
                var otherPlayer = (TicTacToeServerPlayer)otherPeer.PeerData;
                FinishGame(false, otherPlayer);
            }
        }

        public TicTacToeMoveResponse DispatchMoveRequest(TicTacToeMoveRequest moveRequest, TicTacToeServerPlayer player)
        {
            // This method is called from the Application class

            // Check if move is valid
            if (_gameState != GameState.Started || _currentPlayer != player || _board[moveRequest.Position] != default(char))
            {
                return new TicTacToeMoveResponse() { Success = false };
            }

            // Do the move
            ProcessMove(moveRequest.Position, false);
            return new TicTacToeMoveResponse() { Success = true };
        }

        private TicTacToeServerPlayer[] GetPlayers()
        {
            // Helper method to get array of TicTacToe players

            TicTacToeServerPlayer[] players = Peers.Values.Select(peer => peer.PeerData)
                .Cast<TicTacToeServerPlayer>()
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
            var gameStartedEvent = new TicTacToeGameStartEvent() { Player1 = player1.GetPlayerReference(), Player2 = player2.GetPlayerReference(), SecondsPerMove = SecondsPerMove };
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

            ProcessMove(0, true); // Timed out, skip this move
        }

        private void ProcessMove(byte position, bool skipMove)
        {
            var players = GetPlayers();
            var pieceType = _currentPlayer.PieceType;

            // Set board position
            if (!skipMove)
            {
                _board[position] = pieceType;
            }

            // Check if game has ended
            if(IsWinningMove(position, pieceType))
            {
                FinishGame(false, _currentPlayer); // Winner
                return;
            }
            else if (_currentMoveNumber == 9)
            {
                FinishGame(true, null); // Draw
                return;
            }

            // Switch to next move 
            _currentMoveNumber++;
            _currentPlayer = _currentPlayer == players[0] ? players[1] : players[0];


            // Broadcast move end event
            var moveEndEvent = new TicTacToeMoveEvent() { MoveNumber = _currentMoveNumber, Skipped = skipMove, PieceType = _currentPlayer.PieceType, Position = position };
            BroadcastEvent<TicTacToeMoveEvent>(moveEndEvent);

            // Check if game has ended
            ScheduleMoveTimeout();
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

        private void FinishGame(bool isDraw, TicTacToeServerPlayer winner)
        {
            _gameState = GameState.Finished;
            var finishEvent = new TicTacToeGameFinishEvent() { IsDraw = isDraw, Winner = winner?.GetPlayerReference() };
            BroadcastEvent<TicTacToeGameFinishEvent>(finishEvent);
        }

        enum GameState
        {
            WaitingForPlayers,
            Started,
            Finished,
        }
    }
}