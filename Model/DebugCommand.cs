#region Usings

using System;
using System.IO;

#endregion

namespace Aicup2020.Model
{
    public abstract class DebugCommand
    {
        public abstract void WriteTo(BinaryWriter writer);

        public static DebugCommand ReadFrom(BinaryReader reader)
        {
            switch (reader.ReadInt32())
            {
                case Add.TAG:
                    return Add.ReadFrom(reader);
                case Clear.TAG:
                    return Clear.ReadFrom(reader);
                case SetAutoFlush.TAG:
                    return SetAutoFlush.ReadFrom(reader);
                case Flush.TAG:
                    return Flush.ReadFrom(reader);
                default:
                    throw new Exception("Unexpected tag value");
            }
        }

        public class Add : DebugCommand
        {
            public const int TAG = 0;
            public DebugData Data { get; set; }

            public Add()
            {
            }

            public Add(DebugData data)
            {
                Data = data;
            }

            public static new Add ReadFrom(BinaryReader reader)
            {
                var result = new Add();
                result.Data = DebugData.ReadFrom(reader);
                return result;
            }

            public override void WriteTo(BinaryWriter writer)
            {
                writer.Write(TAG);
                Data.WriteTo(writer);
            }
        }

        public class Clear : DebugCommand
        {
            public const int TAG = 1;

            public static new Clear ReadFrom(BinaryReader reader)
            {
                var result = new Clear();
                return result;
            }

            public override void WriteTo(BinaryWriter writer)
            {
                writer.Write(TAG);
            }
        }

        public class SetAutoFlush : DebugCommand
        {
            public const int TAG = 2;
            public bool Enable { get; set; }

            public SetAutoFlush()
            {
            }

            public SetAutoFlush(bool enable)
            {
                Enable = enable;
            }

            public static new SetAutoFlush ReadFrom(BinaryReader reader)
            {
                var result = new SetAutoFlush();
                result.Enable = reader.ReadBoolean();
                return result;
            }

            public override void WriteTo(BinaryWriter writer)
            {
                writer.Write(TAG);
                writer.Write(Enable);
            }
        }

        public class Flush : DebugCommand
        {
            public const int TAG = 3;

            public static new Flush ReadFrom(BinaryReader reader)
            {
                var result = new Flush();
                return result;
            }

            public override void WriteTo(BinaryWriter writer)
            {
                writer.Write(TAG);
            }
        }
    }
}