#region Usings

using System.IO;
using Aicup2020.Model;

#endregion

namespace Aicup2020
{
    public class DebugInterface
    {
        private readonly BinaryWriter writer;
        private readonly BinaryReader reader;

        public DebugInterface(BinaryReader reader, BinaryWriter writer)
        {
            this.reader = reader;
            this.writer = writer;
        }

        public void Send(DebugCommand command)
        {
            new ClientMessage.DebugMessage(command).WriteTo(writer);
            writer.Flush();
        }

        public DebugState GetState()
        {
            new ClientMessage.RequestDebugState().WriteTo(writer);
            writer.Flush();
            return DebugState.ReadFrom(reader);
        }
    }
}