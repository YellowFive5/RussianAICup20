#region Usings

using System.IO;

#endregion

namespace Aicup2020.Model
{
    public struct ColoredVertex
    {
        public Vec2Float? WorldPos { get; set; }
        public Vec2Float ScreenOffset { get; set; }
        public Color Color { get; set; }

        public ColoredVertex(Vec2Float? worldPos, Vec2Float screenOffset, Color color)
        {
            WorldPos = worldPos;
            ScreenOffset = screenOffset;
            Color = color;
        }

        public static ColoredVertex ReadFrom(BinaryReader reader)
        {
            var result = new ColoredVertex();
            if (reader.ReadBoolean())
            {
                result.WorldPos = Vec2Float.ReadFrom(reader);
            }
            else
            {
                result.WorldPos = null;
            }

            result.ScreenOffset = Vec2Float.ReadFrom(reader);
            result.Color = Color.ReadFrom(reader);
            return result;
        }

        public void WriteTo(BinaryWriter writer)
        {
            if (!WorldPos.HasValue)
            {
                writer.Write(false);
            }
            else
            {
                writer.Write(true);
                WorldPos.Value.WriteTo(writer);
            }

            ScreenOffset.WriteTo(writer);
            Color.WriteTo(writer);
        }
    }
}