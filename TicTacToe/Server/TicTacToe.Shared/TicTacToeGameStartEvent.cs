using Fenrir.Multiplayer;

namespace TicTacToe.Shared
{
    /// <summary>
    /// Broadcast to both players when game starts
    /// </summary>
    public class TicTacToeGameStartEvent : IEvent, IByteStreamSerializable
    {
        /// <summary>
        /// First player
        /// </summary>
        public TicTacToePlayerReference Player1 { get; set; }

        /// <summary>
        /// Second player
        /// </summary>
        public TicTacToePlayerReference Player2 { get; set; }

        void IByteStreamSerializable.Deserialize(IByteStreamReader reader)
        {
            Player1 = reader.Read<TicTacToePlayerReference>();
            Player2 = reader.Read<TicTacToePlayerReference>();
        }

        void IByteStreamSerializable.Serialize(IByteStreamWriter writer)
        {
            writer.Write(Player1);
            writer.Write(Player2);
        }
    }
}
