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
        public int PopulationProvide { get; private set; }
        public int PopulationUse { get; private set; }
        public int PopulationFree => PopulationProvide - PopulationUse;
        public bool NeedBuildHouse => PopulationFree <= 1;
        public bool CanBuildHouse => Me.Resource >= HouseBuildingCost;

        #region Costs

        public int WorkerUnitCost { get; private set; }
        public int RangedUnitCost { get; private set; }
        public int MeleeUnitCost { get; private set; }
        public int TurretUnitCost { get; private set; }
        public int HouseBuildingCost { get; private set; }
        public int WorkersBuildingCost { get; private set; }
        public int RangedBuildingCost { get; private set; }
        public int MeleeBuildingCost { get; private set; }
        public int WallBuildingCost { get; private set; }

        #endregion

        public IEnumerable<Entity> SpiceMilange { get; private set; }
        
        public IEnumerable<Vec2Int> FreeSpace { get; private set; }

        #region Enemy

        public IEnumerable<Player> EnemyPlayers { get; private set; }
        public IEnumerable<Entity> EnemyEntities { get; private set; }
        public IEnumerable<Entity> EnemyBuildingsWalls => EnemyEntities.Where(e => e.EntityType == EntityType.Wall).ToArray();
        public IEnumerable<Entity> EnemyBuildingsHouses => EnemyEntities.Where(e => e.EntityType == EntityType.House).ToArray();
        public IEnumerable<Entity> EnemyBuildingsWorkers => EnemyEntities.Where(e => e.EntityType == EntityType.BuilderBase).ToArray();
        public IEnumerable<Entity> EnemyBuildingsMelees => EnemyEntities.Where(e => e.EntityType == EntityType.MeleeBase).ToArray();
        public IEnumerable<Entity> EnemyBuildingsRanged => EnemyEntities.Where(e => e.EntityType == EntityType.RangedBase).ToArray();
        public IEnumerable<Entity> EnemyUnitsTurrets => EnemyEntities.Where(e => e.EntityType == EntityType.Turret).ToArray();
        public IEnumerable<Entity> EnemyUnitsWorkers => EnemyEntities.Where(e => e.EntityType == EntityType.BuilderUnit).ToArray();
        public IEnumerable<Entity> EnemyUnitsMelees => EnemyEntities.Where(e => e.EntityType == EntityType.MeleeUnit).ToArray();
        public IEnumerable<Entity> EnemyUnitsRanged => EnemyEntities.Where(e => e.EntityType == EntityType.RangedUnit).ToArray();

        #endregion

        #region Me

        public Player Me { get; private set; }
        public IEnumerable<Entity> MyEntities { get; private set; }
        public IEnumerable<Entity> MyBuildingsWalls => MyEntities.Where(e => e.EntityType == EntityType.Wall).ToArray();
        public IEnumerable<Entity> MyBuildingsHouses => MyEntities.Where(e => e.EntityType == EntityType.House).ToArray();
        public IEnumerable<Entity> MyBuildingsWorkers => MyEntities.Where(e => e.EntityType == EntityType.BuilderBase).ToArray();
        public IEnumerable<Entity> MyBuildingsMelees => MyEntities.Where(e => e.EntityType == EntityType.MeleeBase).ToArray();
        public IEnumerable<Entity> MyBuildingsRanged => MyEntities.Where(e => e.EntityType == EntityType.RangedBase).ToArray();
        public IEnumerable<Entity> MyUnitsTurrets => MyEntities.Where(e => e.EntityType == EntityType.Turret).ToArray();
        public IEnumerable<Entity> MyUnitsWorkers => MyEntities.Where(e => e.EntityType == EntityType.BuilderUnit).ToArray();
        public IEnumerable<Entity> MyUnitsMelees => MyEntities.Where(e => e.EntityType == EntityType.MeleeUnit).ToArray();
        public IEnumerable<Entity> MyUnitsRanged => MyEntities.Where(e => e.EntityType == EntityType.RangedUnit).ToArray();

        #endregion

        public void Scan(PlayerView view)
        {
            WorkerUnitCost = view.EntityProperties.Single(ep => ep.Key == EntityType.BuilderUnit).Value.Cost;
            RangedUnitCost = view.EntityProperties.Single(ep => ep.Key == EntityType.RangedUnit).Value.Cost;
            HouseBuildingCost = view.EntityProperties.Single(ep => ep.Key == EntityType.House).Value.Cost;
            MeleeUnitCost = view.EntityProperties.Single(ep => ep.Key == EntityType.MeleeUnit).Value.Cost;
            TurretUnitCost = view.EntityProperties.Single(ep => ep.Key == EntityType.Turret).Value.Cost;
            WorkersBuildingCost = view.EntityProperties.Single(ep => ep.Key == EntityType.BuilderBase).Value.Cost;
            RangedBuildingCost = view.EntityProperties.Single(ep => ep.Key == EntityType.RangedBase).Value.Cost;
            MeleeBuildingCost = view.EntityProperties.Single(ep => ep.Key == EntityType.MeleeBase).Value.Cost;
            WallBuildingCost = view.EntityProperties.Single(ep => ep.Key == EntityType.Wall).Value.Cost;

            Me = view.Players.Single(p => p.Id == view.MyId);
            EnemyPlayers = view.Players.Where(p => p.Id != view.MyId).ToArray();
            SpiceMilange = view.Entities.Where(e => e.EntityType == EntityType.Resource).ToArray();
            EnemyEntities = view.Entities.Where(e => e.PlayerId != view.MyId && e.EntityType != EntityType.Resource).ToArray();
            MyEntities = view.Entities.Where(e => e.PlayerId == view.MyId).ToArray();

            PopulationProvide = 0;
            PopulationUse = 0;
            foreach (var entity in MyEntities)
            {
                PopulationProvide += view.EntityProperties.Single(ep => ep.Key == entity.EntityType).Value.PopulationProvide;
                PopulationUse += view.EntityProperties.Single(ep => ep.Key == entity.EntityType).Value.PopulationUse;
            }

            // foreach (var entity in view.Entities)
            // {
            //     view.EntityProperties.Single(ep => ep.Key == entity.EntityType).Value.Size
            // }
        }

        public Entity GetNearestEntityOfType(Entity sourceEntity, PlayerType playerType, EntityType type)
        {
            IEnumerable<Entity> targetCollection;

            switch (playerType)
            {
                case PlayerType.My:
                    switch (type)
                    {
                        case EntityType.Wall:
                            targetCollection = MyBuildingsWalls;
                            break;
                        case EntityType.House:
                            targetCollection = MyBuildingsHouses;
                            break;
                        case EntityType.BuilderBase:
                            targetCollection = MyBuildingsWorkers;
                            break;
                        case EntityType.BuilderUnit:
                            targetCollection = MyUnitsWorkers;
                            break;
                        case EntityType.MeleeBase:
                            targetCollection = MyBuildingsMelees;
                            break;
                        case EntityType.MeleeUnit:
                            targetCollection = MyUnitsMelees;
                            break;
                        case EntityType.RangedBase:
                            targetCollection = MyBuildingsRanged;
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
                            targetCollection = EnemyBuildingsWalls;
                            break;
                        case EntityType.House:
                            targetCollection = EnemyBuildingsHouses;
                            break;
                        case EntityType.BuilderBase:
                            targetCollection = EnemyBuildingsWorkers;
                            break;
                        case EntityType.BuilderUnit:
                            targetCollection = EnemyUnitsWorkers;
                            break;
                        case EntityType.MeleeBase:
                            targetCollection = EnemyBuildingsMelees;
                            break;
                        case EntityType.MeleeUnit:
                            targetCollection = EnemyUnitsMelees;
                            break;
                        case EntityType.RangedBase:
                            targetCollection = EnemyBuildingsRanged;
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

        public Entity GetNearestEntity(Entity sourceEntity, PlayerType playerType)
        {
            IEnumerable<Entity> targetCollection;

            switch (playerType)
            {
                case PlayerType.My:
                    targetCollection = MyEntities;
                    break;
                case PlayerType.Enemy:
                    targetCollection = EnemyEntities;
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