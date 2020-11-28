#region Usings

using System.IO;

#endregion

namespace Aicup2020.Model
{
    public struct Vec2Float
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Vec2Float(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static Vec2Float ReadFrom(BinaryReader reader)
        {
            var result = new Vec2Float();
            result.X = reader.ReadSingle();
            result.Y = reader.ReadSingle();
            return result;
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(X);
            writer.Write(Y);
        }
    }
}