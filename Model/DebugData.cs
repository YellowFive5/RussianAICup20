#region Usings

using System;
using System.IO;
using System.Text;

#endregion

namespace Aicup2020.Model
{
    public abstract class DebugData
    {
        public abstract void WriteTo(BinaryWriter writer);

        public static DebugData ReadFrom(BinaryReader reader)
        {
            switch (reader.ReadInt32())
            {
                case Log.TAG:
                    return Log.ReadFrom(reader);
                case Primitives.TAG:
                    return Primitives.ReadFrom(reader);
                case PlacedText.TAG:
                    return PlacedText.ReadFrom(reader);
                default:
                    throw new Exception("Unexpected tag value");
            }
        }

        public class Log : DebugData
        {
            public const int TAG = 0;
            public string Text { get; set; }

            public Log()
            {
            }

            public Log(string text)
            {
                Text = text;
            }

            public static new Log ReadFrom(BinaryReader reader)
            {
                var result = new Log();
                result.Text = Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt32()));
                return result;
            }

            public override void WriteTo(BinaryWriter writer)
            {
                writer.Write(TAG);
                var TextData = Encoding.UTF8.GetBytes(Text);
                writer.Write(TextData.Length);
                writer.Write(TextData);
            }
        }

        public class Primitives : DebugData
        {
            public const int TAG = 1;
            public ColoredVertex[] Vertices { get; set; }
            public PrimitiveType PrimitiveType { get; set; }

            public Primitives()
            {
            }

            public Primitives(ColoredVertex[] vertices, PrimitiveType primitiveType)
            {
                Vertices = vertices;
                PrimitiveType = primitiveType;
            }

            public static new Primitives ReadFrom(BinaryReader reader)
            {
                var result = new Primitives();
                result.Vertices = new ColoredVertex[reader.ReadInt32()];
                for (int i = 0; i < result.Vertices.Length; i++)
                {
                    result.Vertices[i] = ColoredVertex.ReadFrom(reader);
                }

                switch (reader.ReadInt32())
                {
                    case 0:
                        result.PrimitiveType = PrimitiveType.Lines;
                        break;
                    case 1:
                        result.PrimitiveType = PrimitiveType.Triangles;
                        break;
                    default:
                        throw new Exception("Unexpected tag value");
                }

                return result;
            }

            public override void WriteTo(BinaryWriter writer)
            {
                writer.Write(TAG);
                writer.Write(Vertices.Length);
                foreach (var VerticesElement in Vertices)
                {
                    VerticesElement.WriteTo(writer);
                }

                writer.Write((int) (PrimitiveType));
            }
        }

        public class PlacedText : DebugData
        {
            public const int TAG = 2;
            public ColoredVertex Vertex { get; set; }
            public string Text { get; set; }
            public float Alignment { get; set; }
            public float Size { get; set; }

            public PlacedText()
            {
            }

            public PlacedText(ColoredVertex vertex, string text, float alignment, float size)
            {
                Vertex = vertex;
                Text = text;
                Alignment = alignment;
                Size = size;
            }

            public static new PlacedText ReadFrom(BinaryReader reader)
            {
                var result = new PlacedText();
                result.Vertex = ColoredVertex.ReadFrom(reader);
                result.Text = Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt32()));
                result.Alignment = reader.ReadSingle();
                result.Size = reader.ReadSingle();
                return result;
            }

            public override void WriteTo(BinaryWriter writer)
            {
                writer.Write(TAG);
                Vertex.WriteTo(writer);
                var TextData = Encoding.UTF8.GetBytes(Text);
                writer.Write(TextData.Length);
                writer.Write(TextData);
                writer.Write(Alignment);
                writer.Write(Size);
            }
        }
    }
}