using Fenrir.Multiplayer;

namespace TicTacToe.Shared
{
    /// <summary>
    /// Represents TicTacToe Player Reference 
    /// sent over the wire
    /// </summary>
    public class TicTacToePlayerReference : IByteStreamSerializable
    {
        /// <summary>
        /// Unique Id of the peer
        /// </summary>
        public string PeerId { get; set; }

        /// <summary>
        /// Name of the player
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Type of the piece, 'x' or 'o'
        /// </summary>
        public char PieceType { get; set; }

        void IByteStreamSerializable.Deserialize(IByteStreamReader reader)
        {
            PeerId = reader.ReadString();
            Name = reader.ReadString();
            PieceType = reader.ReadChar();
        }

        void IByteStreamSerializable.Serialize(IByteStreamWriter writer)
        {
            writer.Write(PeerId);
            writer.Write(Name);
            writer.Write(PieceType);
        }
    }
}
