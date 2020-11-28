#region Usings

using System.IO;

#endregion

namespace Aicup2020.Model
{
    public struct MoveAction
    {
        public Vec2Int Target { get; set; }
        public bool FindClosestPosition { get; set; }
        public bool BreakThrough { get; set; }

        public MoveAction(Vec2Int target, bool findClosestPosition, bool breakThrough)
        {
            Target = target;
            FindClosestPosition = findClosestPosition;
            BreakThrough = breakThrough;
        }

        public static MoveAction ReadFrom(BinaryReader reader)
        {
            var result = new MoveAction();
            result.Target = Vec2Int.ReadFrom(reader);
            result.FindClosestPosition = reader.ReadBoolean();
            result.BreakThrough = reader.ReadBoolean();
            return result;
        }

        public void WriteTo(BinaryWriter writer)
        {
            Target.WriteTo(writer);
            writer.Write(FindClosestPosition);
            writer.Write(BreakThrough);
        }
    }
}