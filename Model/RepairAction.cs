#region Usings

using System.IO;

#endregion

namespace Aicup2020.Model
{
    public struct RepairAction
    {
        public int Target { get; set; }

        public RepairAction(int target)
        {
            Target = target;
        }

        public static RepairAction ReadFrom(BinaryReader reader)
        {
            var result = new RepairAction();
            result.Target = reader.ReadInt32();
            return result;
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(Target);
        }
    }
}