#region Usings

using System;
using System.Collections.Generic;
using System.IO;

#endregion

namespace Aicup2020.Model
{
    public struct PlayerView
    {
        public int MyId { get; set; }
        public int MapSize { get; set; }
        public bool FogOfWar { get; set; }
        public IDictionary<EntityType, EntityProperties> EntityProperties { get; set; }
        public int MaxTickCount { get; set; }
        public int MaxPathfindNodes { get; set; }
        public int CurrentTick { get; set; }
        public Player[] Players { get; set; }
        public Entity[] Entities { get; set; }

        public PlayerView(int myId, int mapSize, bool fogOfWar, IDictionary<EntityType, EntityProperties> entityProperties, int maxTickCount, int maxPathfindNodes, int currentTick, Player[] players, Entity[] entities)
        {
            MyId = myId;
            MapSize = mapSize;
            FogOfWar = fogOfWar;
            EntityProperties = entityProperties;
            MaxTickCount = maxTickCount;
            MaxPathfindNodes = maxPathfindNodes;
            CurrentTick = currentTick;
            Players = players;
            Entities = entities;
        }

        public static PlayerView ReadFrom(BinaryReader reader)
        {
            var result = new PlayerView();
            result.MyId = reader.ReadInt32();
            result.MapSize = reader.ReadInt32();
            result.FogOfWar = reader.ReadBoolean();
            int EntityPropertiesSize = reader.ReadInt32();
            result.EntityProperties = new Dictionary<EntityType, EntityProperties>(EntityPropertiesSize);
            for (int i = 0; i < EntityPropertiesSize; i++)
            {
                EntityType EntityPropertiesKey;
                switch (reader.ReadInt32())
                {
                    case 0:
                        EntityPropertiesKey = EntityType.Wall;
                        break;
                    case 1:
                        EntityPropertiesKey = EntityType.House;
                        break;
                    case 2:
                        EntityPropertiesKey = EntityType.BuilderBase;
                        break;
                    case 3:
                        EntityPropertiesKey = EntityType.BuilderUnit;
                        break;
                    case 4:
                        EntityPropertiesKey = EntityType.MeleeBase;
                        break;
                    case 5:
                        EntityPropertiesKey = EntityType.MeleeUnit;
                        break;
                    case 6:
                        EntityPropertiesKey = EntityType.RangedBase;
                        break;
                    case 7:
                        EntityPropertiesKey = EntityType.RangedUnit;
                        break;
                    case 8:
                        EntityPropertiesKey = EntityType.Resource;
                        break;
                    case 9:
                        EntityPropertiesKey = EntityType.Turret;
                        break;
                    default:
                        throw new Exception("Unexpected tag value");
                }

                EntityProperties EntityPropertiesValue;
                EntityPropertiesValue = Model.EntityProperties.ReadFrom(reader);
                result.EntityProperties.Add(EntityPropertiesKey, EntityPropertiesValue);
            }

            result.MaxTickCount = reader.ReadInt32();
            result.MaxPathfindNodes = reader.ReadInt32();
            result.CurrentTick = reader.ReadInt32();
            result.Players = new Player[reader.ReadInt32()];
            for (int i = 0; i < result.Players.Length; i++)
            {
                result.Players[i] = Player.ReadFrom(reader);
            }

            result.Entities = new Entity[reader.ReadInt32()];
            for (int i = 0; i < result.Entities.Length; i++)
            {
                result.Entities[i] = Entity.ReadFrom(reader);
            }

            return result;
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(MyId);
            writer.Write(MapSize);
            writer.Write(FogOfWar);
            writer.Write(EntityProperties.Count);
            foreach (var EntityPropertiesEntry in EntityProperties)
            {
                var EntityPropertiesKey = EntityPropertiesEntry.Key;
                var EntityPropertiesValue = EntityPropertiesEntry.Value;
                writer.Write((int) (EntityPropertiesKey));
                EntityPropertiesValue.WriteTo(writer);
            }

            writer.Write(MaxTickCount);
            writer.Write(MaxPathfindNodes);
            writer.Write(CurrentTick);
            writer.Write(Players.Length);
            foreach (var PlayersElement in Players)
            {
                PlayersElement.WriteTo(writer);
            }

            writer.Write(Entities.Length);
            foreach (var EntitiesElement in Entities)
            {
                EntitiesElement.WriteTo(writer);
            }
        }
    }
}