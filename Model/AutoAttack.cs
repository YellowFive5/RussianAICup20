#region Usings

using System;
using System.IO;

#endregion

namespace Aicup2020.Model
{
    public struct AutoAttack
    {
        public int PathfindRange { get; set; }
        public EntityType[] ValidTargets { get; set; }

        public AutoAttack(int pathfindRange, EntityType[] validTargets)
        {
            PathfindRange = pathfindRange;
            ValidTargets = validTargets;
        }

        public static AutoAttack ReadFrom(BinaryReader reader)
        {
            var result = new AutoAttack();
            result.PathfindRange = reader.ReadInt32();
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

            return result;
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(PathfindRange);
            writer.Write(ValidTargets.Length);
            foreach (var ValidTargetsElement in ValidTargets)
            {
                writer.Write((int) (ValidTargetsElement));
            }
        }
    }
}