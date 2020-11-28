#region Usings

using System.IO;

#endregion

namespace Aicup2020.Model
{
    public struct EntityAction
    {
        public MoveAction? MoveAction { get; set; }
        public BuildAction? BuildAction { get; set; }
        public AttackAction? AttackAction { get; set; }
        public RepairAction? RepairAction { get; set; }

        public EntityAction(MoveAction? moveAction, BuildAction? buildAction, AttackAction? attackAction, RepairAction? repairAction)
        {
            MoveAction = moveAction;
            BuildAction = buildAction;
            AttackAction = attackAction;
            RepairAction = repairAction;
        }

        public static EntityAction ReadFrom(BinaryReader reader)
        {
            var result = new EntityAction();
            if (reader.ReadBoolean())
            {
                result.MoveAction = Model.MoveAction.ReadFrom(reader);
            }
            else
            {
                result.MoveAction = null;
            }

            if (reader.ReadBoolean())
            {
                result.BuildAction = Model.BuildAction.ReadFrom(reader);
            }
            else
            {
                result.BuildAction = null;
            }

            if (reader.ReadBoolean())
            {
                result.AttackAction = Model.AttackAction.ReadFrom(reader);
            }
            else
            {
                result.AttackAction = null;
            }

            if (reader.ReadBoolean())
            {
                result.RepairAction = Model.RepairAction.ReadFrom(reader);
            }
            else
            {
                result.RepairAction = null;
            }

            return result;
        }

        public void WriteTo(BinaryWriter writer)
        {
            if (!MoveAction.HasValue)
            {
                writer.Write(false);
            }
            else
            {
                writer.Write(true);
                MoveAction.Value.WriteTo(writer);
            }

            if (!BuildAction.HasValue)
            {
                writer.Write(false);
            }
            else
            {
                writer.Write(true);
                BuildAction.Value.WriteTo(writer);
            }

            if (!AttackAction.HasValue)
            {
                writer.Write(false);
            }
            else
            {
                writer.Write(true);
                AttackAction.Value.WriteTo(writer);
            }

            if (!RepairAction.HasValue)
            {
                writer.Write(false);
            }
            else
            {
                writer.Write(true);
                RepairAction.Value.WriteTo(writer);
            }
        }
    }
}