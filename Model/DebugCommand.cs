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
    }
}