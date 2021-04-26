using Fenrir.Multiplayer.Network;
using Fenrir.Multiplayer.Serialization;

namespace TicTacToe.Shared
{
    class ExampleEvent : IEvent, IByteStreamSerializable
    {
        public int Value { get; set; }

        public void Deserialize(IByteStreamReader reader)
        {
            Value = reader.ReadInt();
        }

        public void Serialize(IByteStreamWriter writer)
        {
            writer.Write(Value);
        }
    }
}
