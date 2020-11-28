#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Aicup2020.Model;

#endregion

namespace Aicup2020
{
    public class World
    {
        public IEnumerable<Entity> SpiceMilange { get; private set; }

        #region Enemy

        public IEnumerable<Player> EnemyPlayers { get; private set; }
        public IEnumerable<Entity> EnemyEntities { get; private set; }
        public IEnumerable<Entity> EnemyBasesWalls => EnemyEntities.Where(e => e.EntityType == EntityType.Wall).ToArray();
        public IEnumerable<Entity> EnemyBasesHouses => EnemyEntities.Where(e => e.EntityType == EntityType.House).ToArray();
        public IEnumerable<Entity> EnemyBasesBuilders => EnemyEntities.Where(e => e.EntityType == EntityType.BuilderBase).ToArray();
        public IEnumerable<Entity> EnemyBasesMelees => EnemyEntities.Where(e => e.EntityType == EntityType.MeleeBase).ToArray();
        public IEnumerable<Entity> EnemyBasesRanged => EnemyEntities.Where(e => e.EntityType == EntityType.RangedBase).ToArray();
        public IEnumerable<Entity> EnemyUnitsTurrets => EnemyEntities.Where(e => e.EntityType == EntityType.Turret).ToArray();
        public IEnumerable<Entity> EnemyUnitsBuilders => EnemyEntities.Where(e => e.EntityType == EntityType.BuilderUnit).ToArray();
        public IEnumerable<Entity> EnemyUnitsMelees => EnemyEntities.Where(e => e.EntityType == EntityType.MeleeUnit).ToArray();
        public IEnumerable<Entity> EnemyUnitsRanged => EnemyEntities.Where(e => e.EntityType == EntityType.RangedUnit).ToArray();

        #endregion

        #region Me

        public IEnumerable<Entity> MyEntities { get; private set; }
        public IEnumerable<Entity> MyBasesWalls => MyEntities.Where(e => e.EntityType == EntityType.Wall).ToArray();
        public IEnumerable<Entity> MyBasesHouses => MyEntities.Where(e => e.EntityType == EntityType.House).ToArray();
        public IEnumerable<Entity> MyBasesBuilders => MyEntities.Where(e => e.EntityType == EntityType.BuilderBase).ToArray();
        public IEnumerable<Entity> MyBasesMelees => MyEntities.Where(e => e.EntityType == EntityType.MeleeBase).ToArray();
        public IEnumerable<Entity> MyBasesRanged => MyEntities.Where(e => e.EntityType == EntityType.RangedBase).ToArray();
        public IEnumerable<Entity> MyUnitsTurrets => MyEntities.Where(e => e.EntityType == EntityType.Turret).ToArray();
        public IEnumerable<Entity> MyUnitsBuilders => MyEntities.Where(e => e.EntityType == EntityType.BuilderUnit).ToArray();
        public IEnumerable<Entity> MyUnitsMelees => MyEntities.Where(e => e.EntityType == EntityType.MeleeUnit).ToArray();
        public IEnumerable<Entity> MyUnitsRanged => MyEntities.Where(e => e.EntityType == EntityType.RangedUnit).ToArray();

        #endregion

        public void Scan(PlayerView view)
        {
            EnemyPlayers = view.Players.Where(p => p.Id != view.MyId).ToArray();
            SpiceMilange = view.Entities.Where(e => e.EntityType == EntityType.Resource).ToArray();
            EnemyEntities = view.Entities.Where(e => e.PlayerId != view.MyId && e.EntityType != EntityType.Resource).ToArray();
            MyEntities = view.Entities.Where(e => e.PlayerId == view.MyId).ToArray();
        }

        public Entity GetNearestTo(Entity sourceEntity, PlayerType playerType, EntityType type)
        {
            IEnumerable<Entity> targetCollection;

            switch (playerType)
            {
                case PlayerType.My:
                    switch (type)
                    {
                        case EntityType.Wall:
                            targetCollection = MyBasesWalls;
                            break;
                        case EntityType.House:
                            targetCollection = MyBasesHouses;
                            break;
                        case EntityType.BuilderBase:
                            targetCollection = MyBasesBuilders;
                            break;
                        case EntityType.BuilderUnit:
                            targetCollection = MyUnitsBuilders;
                            break;
                        case EntityType.MeleeBase:
                            targetCollection = MyBasesMelees;
                            break;
                        case EntityType.MeleeUnit:
                            targetCollection = MyUnitsMelees;
                            break;
                        case EntityType.RangedBase:
                            targetCollection = MyBasesRanged;
                            break;
                        case EntityType.RangedUnit:
                            targetCollection = MyUnitsRanged;
                            break;
                        case EntityType.Resource:
                            targetCollection = SpiceMilange;
                            break;
                        case EntityType.Turret:
                            targetCollection = MyUnitsTurrets;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(type), type, null);
                    }

                    break;
                case PlayerType.Enemy:
                    switch (type)
                    {
                        case EntityType.Wall:
                            targetCollection = EnemyBasesWalls;
                            break;
                        case EntityType.House:
                            targetCollection = EnemyBasesHouses;
                            break;
                        case EntityType.BuilderBase:
                            targetCollection = EnemyBasesBuilders;
                            break;
                        case EntityType.BuilderUnit:
                            targetCollection = EnemyUnitsBuilders;
                            break;
                        case EntityType.MeleeBase:
                            targetCollection = EnemyBasesMelees;
                            break;
                        case EntityType.MeleeUnit:
                            targetCollection = EnemyUnitsMelees;
                            break;
                        case EntityType.RangedBase:
                            targetCollection = EnemyBasesRanged;
                            break;
                        case EntityType.RangedUnit:
                            targetCollection = EnemyUnitsRanged;
                            break;
                        case EntityType.Resource:
                            targetCollection = SpiceMilange;
                            break;
                        case EntityType.Turret:
                            targetCollection = EnemyUnitsTurrets;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(type), type, null);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(playerType), playerType, null);
            }

            double distanceBetween = 1000;
            var nearestEntity = new Entity();

            foreach (var ett in targetCollection) // todo to LINQ
            {
                var dst = GetDistance(ett.Position, sourceEntity.Position);
                if (dst < distanceBetween)
                {
                    distanceBetween = dst;
                    nearestEntity = ett;
                }
            }

            return nearestEntity;
        }

        private double GetDistance(Vec2Int one, Vec2Int two)
        {
            var distX = two.X - one.X;
            var distY = two.Y - one.Y;
            return Math.Sqrt(distX * distX + distY * distY);
        }
    }
}