using Fenrir.Multiplayer.Network;
using Fenrir.Multiplayer.Serialization;

namespace TicTacToe.Shared
{
    /// <summary>
    /// Broadcast to both players when move is made
    /// </summary>
    public class TicTacToeMoveEvent : IEvent, IByteStreamSerializable
    {
        /// <summary>
        /// Number of the move
        /// </summary>
        public int MoveNumber { get; set; }

        /// <summary>
        /// If true, player did not make the move in time
        /// </summary>
        public bool Skipped { get; set; }

        /// <summary>
        /// Move position on the board.
        /// Index of the cell 0-9
        /// </summary>
        public byte Position { get; set; }

        /// <summary>
        /// Type of the piece, 'x' or 'o'
        /// </summary>
        public char PieceType { get; set; }

        void IByteStreamSerializable.Deserialize(IByteStreamReader reader)
        {
            Position = reader.ReadByte();
            PieceType = reader.ReadChar();
        }

        void IByteStreamSerializable.Serialize(IByteStreamWriter writer)
        {
            writer.Write(Position);
            writer.Write(PieceType);
        }
    }
}
