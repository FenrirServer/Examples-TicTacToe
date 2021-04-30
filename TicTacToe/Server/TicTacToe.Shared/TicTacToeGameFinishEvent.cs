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
        /// If true, game ended with a draw
        /// </summary>
        public bool IsDraw { get; set; }

        /// <summary>
        /// Reference to the play who won the game
        /// </summary>
        public TicTacToePlayerReference Winner { get; set; }

        void IByteStreamSerializable.Deserialize(IByteStreamReader reader)
        {
            IsDraw = reader.ReadBool();

            if (!IsDraw) 
            {
                Winner = reader.Read<TicTacToePlayerReference>();
            }
        }

        void IByteStreamSerializable.Serialize(IByteStreamWriter writer)
        {
            writer.Write(IsDraw);

            if (!IsDraw)
            {
                writer.Write(Winner);
            }
        }
    }
}
