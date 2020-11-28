#region Usings

using System.IO;

#endregion

namespace Aicup2020.Model
{
    public struct Camera
    {
        public Vec2Float Center { get; set; }
        public float Rotation { get; set; }
        public float Attack { get; set; }
        public float Distance { get; set; }
        public bool Perspective { get; set; }

        public Camera(Vec2Float center, float rotation, float attack, float distance, bool perspective)
        {
            Center = center;
            Rotation = rotation;
            Attack = attack;
            Distance = distance;
            Perspective = perspective;
        }

        public static Camera ReadFrom(BinaryReader reader)
        {
            var result = new Camera();
            result.Center = Vec2Float.ReadFrom(reader);
            result.Rotation = reader.ReadSingle();
            result.Attack = reader.ReadSingle();
            result.Distance = reader.ReadSingle();
            result.Perspective = reader.ReadBoolean();
            return result;
        }

        public void WriteTo(BinaryWriter writer)
        {
            Center.WriteTo(writer);
            writer.Write(Rotation);
            writer.Write(Attack);
            writer.Write(Distance);
            writer.Write(Perspective);
        }
    }
}