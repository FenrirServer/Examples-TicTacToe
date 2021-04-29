using Fenrir.Multiplayer.Network;
using Fenrir.Multiplayer.Serialization;

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

        /// <summary>
        /// Seconds allowed per move
        /// </summary>
        public float SecondsPerMove { get; set; }

        void IByteStreamSerializable.Deserialize(IByteStreamReader reader)
        {
            Player1 = reader.Read<TicTacToePlayerReference>();
            Player2 = reader.Read<TicTacToePlayerReference>();
            SecondsPerMove = reader.ReadFloat();
        }

        void IByteStreamSerializable.Serialize(IByteStreamWriter writer)
        {
            writer.Write(Player1);
            writer.Write(Player2);
            writer.Write(SecondsPerMove);
        }
    }
}
