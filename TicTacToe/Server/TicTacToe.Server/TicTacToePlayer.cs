using Fenrir.Multiplayer.Network;

namespace TicTacToe.Server
{
    class TicTacToePlayer
    {
        /// <summary>
        /// Connection peer
        /// </summary>
        public IServerPeer Peer { get; private set; }

        /// <summary>
        /// Room that this player is assigned to.
        /// Currently player can only be in one room
        /// </summary>
        public TicTacToeRoom Room { get; private set; }

        /// <summary>
        /// Name of the player
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Type of the piece, 'x' or 'o'
        /// </summary>
        public char PieceType { get; set; }

        public TicTacToePlayer(IServerPeer peer, TicTacToeRoom room, string name)
        {
            Peer = peer;
            Room = room;
            Name = name;
        }
    }
}
