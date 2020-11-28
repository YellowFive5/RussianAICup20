#region Usings

using System;
using System.IO;

#endregion

namespace Aicup2020.Model
{
    public struct RepairProperties
    {
        public EntityType[] ValidTargets { get; set; }
        public int Power { get; set; }

        public RepairProperties(EntityType[] validTargets, int power)
        {
            ValidTargets = validTargets;
            Power = power;
        }

        public static RepairProperties ReadFrom(BinaryReader reader)
        {
            var result = new RepairProperties();
            result.ValidTargets = new EntityType[reader.ReadInt32()];
            for (int i = 0; i < result.ValidTargets.Length; i++)
            {
                switch (reader.ReadInt32())
                {
                    case 0:
                        result.ValidTargets[i] = EntityType.Wall;
                        break;
                    case 1:
                        result.ValidTargets[i] = EntityType.House;
                        break;
                    case 2:
                        result.ValidTargets[i] = EntityType.BuilderBase;
                        break;
                    case 3:
                        result.ValidTargets[i] = EntityType.BuilderUnit;
                        break;
                    case 4:
                        result.ValidTargets[i] = EntityType.MeleeBase;
                        break;
                    case 5:
                        result.ValidTargets[i] = EntityType.MeleeUnit;
                        break;
                    case 6:
                        result.ValidTargets[i] = EntityType.RangedBase;
                        break;
                    case 7:
                        result.ValidTargets[i] = EntityType.RangedUnit;
                        break;
                    case 8:
                        result.ValidTargets[i] = EntityType.Resource;
                        break;
                    case 9:
                        result.ValidTargets[i] = EntityType.Turret;
                        break;
                    default:
                        throw new Exception("Unexpected tag value");
                }
            }

            result.Power = reader.ReadInt32();
            return result;
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(ValidTargets.Length);
            foreach (var ValidTargetsElement in ValidTargets)
            {
                writer.Write((int) (ValidTargetsElement));
            }

            writer.Write(Power);
        }
    }
}