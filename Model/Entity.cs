#region Usings

using System;
using System.IO;

#endregion

namespace Aicup2020.Model
{
    public struct Entity
    {
        public int Id { get; set; }
        public int? PlayerId { get; set; }
        public EntityType EntityType { get; set; }
        public Vec2Int Position { get; set; }
        public int Health { get; set; }
        public bool Active { get; set; }

        public Entity(int id, int? playerId, EntityType entityType, Vec2Int position, int health, bool active)
        {
            Id = id;
            PlayerId = playerId;
            EntityType = entityType;
            Position = position;
            Health = health;
            Active = active;
        }

        public static Entity ReadFrom(BinaryReader reader)
        {
            var result = new Entity();
            result.Id = reader.ReadInt32();
            if (reader.ReadBoolean())
            {
                result.PlayerId = reader.ReadInt32();
            }
            else
            {
                result.PlayerId = null;
            }

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
            result.Health = reader.ReadInt32();
            result.Active = reader.ReadBoolean();
            return result;
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(Id);
            if (!PlayerId.HasValue)
            {
                writer.Write(false);
            }
            else
            {
                writer.Write(true);
                writer.Write(PlayerId.Value);
            }

            writer.Write((int) (EntityType));
            Position.WriteTo(writer);
            writer.Write(Health);
            writer.Write(Active);
        }
    }
}