#region Usings

using System;
using System.IO;

#endregion

namespace Aicup2020.Model
{
    public abstract class ClientMessage
    {
        public abstract void WriteTo(BinaryWriter writer);

        public static ClientMessage ReadFrom(BinaryReader reader)
        {
            switch (reader.ReadInt32())
            {
                case DebugMessage.TAG:
                    return DebugMessage.ReadFrom(reader);
                case ActionMessage.TAG:
                    return ActionMessage.ReadFrom(reader);
                case DebugUpdateDone.TAG:
                    return DebugUpdateDone.ReadFrom(reader);
                case RequestDebugState.TAG:
                    return RequestDebugState.ReadFrom(reader);
                default:
                    throw new Exception("Unexpected tag value");
            }
        }

        public class DebugMessage : ClientMessage
        {
            public const int TAG = 0;
            public DebugCommand Command { get; set; }

            public DebugMessage()
            {
            }

            public DebugMessage(DebugCommand command)
            {
                Command = command;
            }

            public static new DebugMessage ReadFrom(BinaryReader reader)
            {
                var result = new DebugMessage();
                result.Command = DebugCommand.ReadFrom(reader);
                return result;
            }

            public override void WriteTo(BinaryWriter writer)
            {
                writer.Write(TAG);
                Command.WriteTo(writer);
            }
        }

        public class ActionMessage : ClientMessage
        {
            public const int TAG = 1;
            public Action Action { get; set; }

            public ActionMessage()
            {
            }

            public ActionMessage(Action action)
            {
                Action = action;
            }

            public static new ActionMessage ReadFrom(BinaryReader reader)
            {
                var result = new ActionMessage();
                result.Action = Action.ReadFrom(reader);
                return result;
            }

            public override void WriteTo(BinaryWriter writer)
            {
                writer.Write(TAG);
                Action.WriteTo(writer);
            }
        }

        public class DebugUpdateDone : ClientMessage
        {
            public const int TAG = 2;

            public static new DebugUpdateDone ReadFrom(BinaryReader reader)
            {
                var result = new DebugUpdateDone();
                return result;
            }

            public override void WriteTo(BinaryWriter writer)
            {
                writer.Write(TAG);
            }
        }

        public class RequestDebugState : ClientMessage
        {
            public const int TAG = 3;

            public static new RequestDebugState ReadFrom(BinaryReader reader)
            {
                var result = new RequestDebugState();
                return result;
            }

            public override void WriteTo(BinaryWriter writer)
            {
                writer.Write(TAG);
            }
        }
    }
}