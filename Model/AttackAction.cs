#region Usings

using System.IO;

#endregion

namespace Aicup2020.Model
{
    public struct AttackAction
    {
        public int? Target { get; set; }
        public AutoAttack? AutoAttack { get; set; }

        public AttackAction(int? target, AutoAttack? autoAttack)
        {
            Target = target;
            AutoAttack = autoAttack;
        }

        public static AttackAction ReadFrom(BinaryReader reader)
        {
            var result = new AttackAction();
            if (reader.ReadBoolean())
            {
                result.Target = reader.ReadInt32();
            }
            else
            {
                result.Target = null;
            }

            if (reader.ReadBoolean())
            {
                result.AutoAttack = Model.AutoAttack.ReadFrom(reader);
            }
            else
            {
                result.AutoAttack = null;
            }

            return result;
        }

        public void WriteTo(BinaryWriter writer)
        {
            if (!Target.HasValue)
            {
                writer.Write(false);
            }
            else
            {
                writer.Write(true);
                writer.Write(Target.Value);
            }

            if (!AutoAttack.HasValue)
            {
                writer.Write(false);
            }
            else
            {
                writer.Write(true);
                AutoAttack.Value.WriteTo(writer);
            }
        }
    }
}