using Fenrir.Multiplayer;

namespace TicTacToe.Shared
{
    /// <summary>
    /// Request to make a move
    /// </summary>
    public class TicTacToeMoveRequest : IRequest<TicTacToeMoveResponse>, IByteStreamSerializable
    {
        /// <summary>
        /// Move position on the board.
        /// Index of the cell 0-9
        /// </summary>
        public byte Position { get; set; }

        void IByteStreamSerializable.Deserialize(IByteStreamReader reader)
        {
            Position = reader.ReadByte();
        }

        void IByteStreamSerializable.Serialize(IByteStreamWriter writer)
        {
            writer.Write(Position);
        }
    }
}
