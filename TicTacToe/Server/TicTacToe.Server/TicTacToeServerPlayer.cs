using Fenrir.Multiplayer;
using TicTacToe.Shared;

namespace TicTacToe.Server
{
    class TicTacToeServerPlayer
    {
        /// <summary>
        /// Connection peer
        /// </summary>
        public IServerPeer Peer { get; private set; }

        /// <summary>
        /// Room that this player is assigned to.
        /// Currently player can only be in one room
        /// </summary>
        public TicTacToeServerRoom Room { get; private set; }

        /// <summary>
        /// Name of the player
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Type of the piece, 'x' or 'o'
        /// </summary>
        public char PieceType { get; set; }

        public TicTacToeServerPlayer(IServerPeer peer, TicTacToeServerRoom room, string name)
        {
            Peer = peer;
            Room = room;
            Name = name;
        }

        public TicTacToePlayerReference GetPlayerReference()
        {
            return new TicTacToePlayerReference() { PeerId = Peer.Id, Name = Name, PieceType = PieceType };
        }
    }
}
