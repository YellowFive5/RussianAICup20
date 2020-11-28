#region Usings

using System.IO;

#endregion

namespace Aicup2020.Model
{
    public struct Vec2Int
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Vec2Int(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Vec2Int ReadFrom(BinaryReader reader)
        {
            var result = new Vec2Int();
            result.X = reader.ReadInt32();
            result.Y = reader.ReadInt32();
            return result;
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(X);
            writer.Write(Y);
        }
    }
}