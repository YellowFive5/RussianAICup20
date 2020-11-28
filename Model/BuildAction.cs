#region Usings

using System;
using System.IO;

#endregion

namespace Aicup2020.Model
{
    public struct BuildAction
    {
        public EntityType EntityType { get; set; }
        public Vec2Int Position { get; set; }

        public BuildAction(EntityType entityType, Vec2Int position)
        {
            EntityType = entityType;
            Position = position;
        }

        public static BuildAction ReadFrom(BinaryReader reader)
        {
            var result = new BuildAction();
            switch (reader.ReadInt32())
            {
                case 0:
                    result.EntityType = EntityType.Wall;
                    break;
                case 1:
                    result.EntityType = EntityType.House;
                    break;
                case 2:
                    result.EntityType = EntityType.BuilderBase;
                    break;
                case 3:
                    result.EntityType = EntityType.BuilderUnit;
                    break;
                case 4:
                    result.EntityType = EntityType.MeleeBase;
                    break;
                case 5:
                    result.EntityType = EntityType.MeleeUnit;
                    break;
                case 6:
                    result.EntityType = EntityType.RangedBase;
                    break;
                case 7:
                    result.EntityType = EntityType.RangedUnit;
                    break;
                case 8:
                    result.EntityType = EntityType.Resource;
                    break;
                case 9:
                    result.EntityType = EntityType.Turret;
                    break;
                default:
                    throw new Exception("Unexpected tag value");
            }

            result.Position = Vec2Int.ReadFrom(reader);
            return result;
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write((int) (EntityType));
            Position.WriteTo(writer);
        }
    }
}