#region Usings

using System.IO;

#endregion

namespace Aicup2020.Model
{
    public struct Color
    {
        public float R { get; set; }
        public float G { get; set; }
        public float B { get; set; }
        public float A { get; set; }

        public Color(float r, float g, float b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public static Color ReadFrom(BinaryReader reader)
        {
            var result = new Color();
            result.R = reader.ReadSingle();
            result.G = reader.ReadSingle();
            result.B = reader.ReadSingle();
            result.A = reader.ReadSingle();
            return result;
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(R);
            writer.Write(G);
            writer.Write(B);
            writer.Write(A);
        }
    }
}