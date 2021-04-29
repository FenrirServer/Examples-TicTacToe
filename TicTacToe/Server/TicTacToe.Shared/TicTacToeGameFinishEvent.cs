using Fenrir.Multiplayer.Network;
using Fenrir.Multiplayer.Serialization;

namespace TicTacToe.Shared
{
    /// <summary>
    /// Broadcast to both players when game finishes
    /// </summary>
    public class TicTacToeGameFinishEvent : IEvent, IByteStreamSerializable
    {
        /// <summary>
        /// Reference to the play who won the game
        /// </summary>
        public TicTacToePlayerReference Winner { get; set; }

        void IByteStreamSerializable.Deserialize(IByteStreamReader reader)
        {
            Winner = reader.Read<TicTacToePlayerReference>();
        }

        void IByteStreamSerializable.Serialize(IByteStreamWriter writer)
        {
            writer.Write(Winner);
        }
    }
}
