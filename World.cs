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
        public BehaviorType Behavior { get; private set; }
        public Vec2Int SquareOfMyInterests { get; private set; }

        public readonly double[] aggressiveBehaviorUnitsRatio = {0.2, 0.8, 0.0};
        public readonly double[] passiveBehaviorUnitsRatio = {0.8, 0.2, 0.0};
        public double[] unitsRatio;
        public int PopulationProvide { get; private set; }
        public int PopulationUse { get; private set; }
        public int PopulationFree => PopulationProvide - PopulationUse;

        public bool NeedBuildBuildingWorkers => !MyBuildingsWorkers.Any() &&
                                                Me.Resource >= WorkersBuildingCost;

        public bool NeedBuildBuildingRanged => !MyBuildingsRanged.Any() &&
                                               Me.Resource >= RangedBuildingCost &&
                                               !NeedBuildBuildingWorkers;

        public bool NeedBuildHouse => PopulationFree <= 1 &&
                                      Me.Resource >= HouseBuildingCost &&
                                      !NeedBuildBuildingWorkers &&
                                      !NeedBuildBuildingRanged;

        public bool RepairNeeds => MyBuildingsBroken.Any();

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

        public Vec2Int Zero;
        public List<Entity> SpiceMilange { get; private set; }
        public List<Entity> BusySpiceMilange { get; private set; }

        // public List<Vec2Int> Points { get; }

        public List<Vec2Int> NotFreePoints { get; private set; }

        #region Enemy

        public IEnumerable<Player> EnemyPlayers { get; private set; }
        public IEnumerable<Entity> EnemyEntities { get; private set; }
        public IEnumerable<Entity> EnemyBuildings => EnemyEntities.Where(e => e.EntityType == EntityType.House || e.EntityType == EntityType.BuilderBase || e.EntityType == EntityType.MeleeBase || e.EntityType == EntityType.RangedBase || e.EntityType == EntityType.Turret);

        public IEnumerable<Entity> EnemyBuildingsWalls => EnemyEntities.Where(e => e.EntityType == EntityType.Wall);
        public IEnumerable<Entity> EnemyBuildingsHouses => EnemyEntities.Where(e => e.EntityType == EntityType.House);
        public IEnumerable<Entity> EnemyBuildingsWorkers => EnemyEntities.Where(e => e.EntityType == EntityType.BuilderBase);
        public IEnumerable<Entity> EnemyBuildingsMelees => EnemyEntities.Where(e => e.EntityType == EntityType.MeleeBase);
        public IEnumerable<Entity> EnemyBuildingsRanged => EnemyEntities.Where(e => e.EntityType == EntityType.RangedBase);
        public IEnumerable<Entity> EnemyUnits => EnemyEntities.Where(e => e.EntityType == EntityType.BuilderUnit || e.EntityType == EntityType.MeleeUnit || e.EntityType == EntityType.RangedUnit || e.EntityType == EntityType.Turret);

        public IEnumerable<Entity> EnemyUnitsTurrets => EnemyEntities.Where(e => e.EntityType == EntityType.Turret);
        public IEnumerable<Entity> EnemyUnitsWorkers => EnemyEntities.Where(e => e.EntityType == EntityType.BuilderUnit);
        public IEnumerable<Entity> EnemyUnitsMelees => EnemyEntities.Where(e => e.EntityType == EntityType.MeleeUnit);
        public IEnumerable<Entity> EnemyUnitsRanged => EnemyEntities.Where(e => e.EntityType == EntityType.RangedUnit);

        #endregion

        #region Me

        public Player Me { get; private set; }
        public IEnumerable<Entity> MyEntities { get; private set; }
        public IEnumerable<Entity> MyBuildings => MyEntities.Where(e => e.EntityType == EntityType.House || e.EntityType == EntityType.BuilderBase || e.EntityType == EntityType.MeleeBase || e.EntityType == EntityType.RangedBase || e.EntityType == EntityType.Turret);
        public IEnumerable<Entity> MyBuildingsBroken { get; private set; }
        public IEnumerable<Entity> MyUnitsBroken { get; private set; }
        public IEnumerable<Entity> MyBuildingsWalls => MyEntities.Where(e => e.EntityType == EntityType.Wall);
        public IEnumerable<Entity> MyBuildingsHouses => MyEntities.Where(e => e.EntityType == EntityType.House);
        public IEnumerable<Entity> MyBuildingsWorkers => MyEntities.Where(e => e.EntityType == EntityType.BuilderBase);
        public IEnumerable<Entity> MyBuildingsMelees => MyEntities.Where(e => e.EntityType == EntityType.MeleeBase);
        public IEnumerable<Entity> MyBuildingsRanged => MyEntities.Where(e => e.EntityType == EntityType.RangedBase);
        public IEnumerable<Entity> MyUnits => MyEntities.Where(e => e.EntityType == EntityType.BuilderUnit || e.EntityType == EntityType.MeleeUnit || e.EntityType == EntityType.RangedUnit || e.EntityType == EntityType.Turret);
        public IEnumerable<Entity> MyUnitsTurrets => MyEntities.Where(e => e.EntityType == EntityType.Turret);

        public IEnumerable<Entity> MyUnitsWorkers => MyEntities.Where(e => e.EntityType == EntityType.BuilderUnit);

        // public List<Entity> MyUnitsBusyWorkers { get; private set; }
        public IEnumerable<Entity> MyUnitsMelees => MyEntities.Where(e => e.EntityType == EntityType.MeleeUnit);
        public IEnumerable<Entity> MyUnitsRanged => MyEntities.Where(e => e.EntityType == EntityType.RangedUnit);

        #endregion

        public World()
        {
            Zero = new Vec2Int(0, 0);
            // Points = new List<Vec2Int>();
            // for (int x = 0; x < 80; x++)
            // {
            //     for (int y = 0; y < 80; y++)
            //     {
            //         Points.Add(new Vec2Int(x, y));
            //     }
            // }
        }

        public void Scan(PlayerView view)
        {
            Me = view.Players.Single(p => p.Id == view.MyId);
            EnemyPlayers = view.Players.Where(p => p.Id != view.MyId).ToList();
            SpiceMilange = view.Entities.Where(e => e.EntityType == EntityType.Resource).ToList();
            BusySpiceMilange = new List<Entity>();
            EnemyEntities = view.Entities.Where(e => e.PlayerId != view.MyId && e.EntityType != EntityType.Resource).ToList();
            MyEntities = view.Entities.Where(e => e.PlayerId == view.MyId).ToList();
            MyBuildingsBroken = MyBuildings.Where(b => b.Health < view.EntityProperties.Single(ep => ep.Key == b.EntityType).Value.MaxHealth).ToList();
            MyUnitsBroken = MyUnits.Where(b => b.Health < view.EntityProperties.Single(ep => ep.Key == b.EntityType).Value.MaxHealth).ToList();
            // MyUnitsBusyWorkers = new List<Entity>();

            // todo delete -1 in another round
            HouseBuildingCost = view.EntityProperties.Single(ep => ep.Key == EntityType.House).Value.InitialCost + MyBuildingsHouses.Count();
            WorkersBuildingCost = view.EntityProperties.Single(ep => ep.Key == EntityType.BuilderBase).Value.InitialCost + MyBuildingsWorkers.Count() - 1;
            RangedBuildingCost = view.EntityProperties.Single(ep => ep.Key == EntityType.RangedBase).Value.InitialCost + MyBuildingsRanged.Count() - 1;
            MeleeBuildingCost = view.EntityProperties.Single(ep => ep.Key == EntityType.MeleeBase).Value.InitialCost + MyBuildingsMelees.Count() - 1;
            WallBuildingCost = view.EntityProperties.Single(ep => ep.Key == EntityType.Wall).Value.InitialCost + MyBuildingsWalls.Count();
            WorkerUnitCost = view.EntityProperties.Single(ep => ep.Key == EntityType.BuilderUnit).Value.InitialCost + MyUnitsWorkers.Count() - 1;
            RangedUnitCost = view.EntityProperties.Single(ep => ep.Key == EntityType.RangedUnit).Value.InitialCost + MyUnitsRanged.Count() - 1;
            MeleeUnitCost = view.EntityProperties.Single(ep => ep.Key == EntityType.MeleeUnit).Value.InitialCost + MyUnitsMelees.Count() - 1;
            TurretUnitCost = view.EntityProperties.Single(ep => ep.Key == EntityType.Turret).Value.InitialCost + MyUnitsTurrets.Count() - 1;

            // SquareOfMyInterests = new Vec2Int(MyEntities.Max(e => e.Position.X) + 1,
            //                                   MyEntities.Max(e => e.Position.Y) + 1);
            SquareOfMyInterests = new Vec2Int(35, 35);
            PopulationProvide = 0;
            PopulationUse = 0;
            foreach (var entity in MyEntities)
            {
                PopulationProvide += view.EntityProperties.Single(ep => ep.Key == entity.EntityType).Value.PopulationProvide;
                PopulationUse += view.EntityProperties.Single(ep => ep.Key == entity.EntityType).Value.PopulationUse;
            }

            NotFreePoints = new List<Vec2Int>();
            foreach (var entity in view.Entities)
            {
                var size = view.EntityProperties.Single(ep => ep.Key == entity.EntityType).Value.Size;

                if (MyBuildings.Contains(entity))
                {
                    for (var x = 0; x < size + 2; x++)
                    {
                        for (var y = 0; y < size + 2; y++)
                        {
                            var _x = entity.Position.X + x - 1;
                            var _y = entity.Position.Y + y - 1;
                            NotFreePoints.CheckPointInsideAndSave(_x, _y);
                        }
                    }
                }
                else
                {
                    for (var x = 0; x < size; x++)
                    {
                        for (var y = 0; y < size; y++)
                        {
                            NotFreePoints.Add(new Vec2Int(entity.Position.X + x,
                                                          entity.Position.Y + y));
                        }
                    }
                }
            }

            ChooseBehavior();
        }

        private void ChooseBehavior()
        {
            if (EnemyEntities.Any() &&
                MyEntities.Any(e => GetDistance(GetNearestEntity(e.Position, PlayerType.Enemy).Position, e.Position) < 6))
                // ||
                // EnemyUnits.Any(eu => GetDistance(eu.Position, Zero) <= 35))
            {
                Behavior = BehaviorType.Aggressive;
                unitsRatio = aggressiveBehaviorUnitsRatio;
            }
            else
            {
                Behavior = BehaviorType.Passive;
                unitsRatio = passiveBehaviorUnitsRatio;
            }
        }

        public Entity GetNearestEntityOfType(Vec2Int sourcePosition, PlayerType playerType, EntityType type, int position = 0)
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
                var dst = GetDistance(ett.Position, sourcePosition);
                if (dst < distanceBetween)
                {
                    distanceBetween = dst;
                    nearestEntity = ett;
                }
            }

            return nearestEntity;
        }

        public Entity GetNearestEntity(Vec2Int sourcePosition, PlayerType playerType)
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
                var dst = GetDistance(ett.Position, sourcePosition);
                if (dst < distanceBetween)
                {
                    distanceBetween = dst;
                    nearestEntity = ett;
                }
            }

            return nearestEntity;
        }

        public Entity GetNearestNotBusySpice(Vec2Int sourcePosition)
        {
            var targetCollection = SpiceMilange.Except(BusySpiceMilange);

            double distanceBetween = 1000;
            var nearestEntity = new Entity();

            foreach (var ett in targetCollection) // todo to LINQ
            {
                var dst = GetDistance(ett.Position, sourcePosition);
                if (dst < distanceBetween)
                {
                    distanceBetween = dst;
                    nearestEntity = ett;
                }
            }

            BusySpiceMilange.Add(nearestEntity);
            return nearestEntity;
        }

        public Entity GetNearestWorker(Vec2Int sourcePosition)
        {
            var nearestBuilder = GetNearestEntityOfType(sourcePosition, PlayerType.My, EntityType.BuilderUnit);

            // var nearestEntity = MyUnitsWorkers.Except(MyUnitsBusyWorkers)
            //                                   .OrderBy(e => GetDistance(e.Position, sourcePosition))
            //                                   .First();
            //
            // MyUnitsBusyWorkers.Add(nearestEntity);
            return nearestBuilder;
        }

        private double GetDistance(Vec2Int one, Vec2Int two)
        {
            var distX = two.X - one.X;
            var distY = two.Y - one.Y;
            return Math.Sqrt(distX * distX + distY * distY);
        }
    }
}