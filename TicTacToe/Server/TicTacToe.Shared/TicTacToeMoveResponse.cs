using Fenrir.Multiplayer.Network;
using Fenrir.Multiplayer.Serialization;

namespace TicTacToe.Shared
{
    /// <summary>
    /// Sent as a response to a player move
    /// </summary>
    public class TicTacToeMoveResponse : IResponse, IByteStreamSerializable
    {
        /// <summary>
        /// Indicates if move was successful
        /// </summary>
        public bool Success { get; set; }

        void IByteStreamSerializable.Deserialize(IByteStreamReader reader)
        {
            Success = reader.ReadBool();
        }

        void IByteStreamSerializable.Serialize(IByteStreamWriter writer)
        {
            writer.Write(Success);
        }
    }
}
