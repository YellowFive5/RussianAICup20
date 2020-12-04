#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Aicup2020.Model;
using Action = Aicup2020.Model.Action;

#endregion

namespace Aicup2020
{
    public class MyStrategy
    {
        private PlayerView View { get; set; }
        private DebugInterface Debug { get; set; }
        private World Around { get; }
        private Dictionary<int, EntityAction> actions;
        private readonly List<int[]> positionsToBuildAround5;
        private readonly List<int[]> positionsToBuildAround3;
        private readonly List<int[]> positionsToBuildAround2;

        public MyStrategy()
        {
            Around = new World();
            positionsToBuildAround5 = new List<int[]>
                                      {
                                          new[] {-1, -1},
                                          new[] {0, -1},
                                          new[] {1, -1},
                                          new[] {2, -1},
                                          new[] {3, -1},
                                          new[] {4, -1},
                                          new[] {5, -1},

                                          new[] {-1, 0},
                                          new[] {-1, 1},
                                          new[] {-1, 2},
                                          new[] {-1, 3},
                                          new[] {-1, 4},
                                          new[] {-1, 5},

                                          new[] {0, 5},
                                          new[] {1, 5},
                                          new[] {2, 5},
                                          new[] {3, 5},
                                          new[] {4, 5},
                                          new[] {5, 5},

                                          new[] {5, 4},
                                          new[] {5, 3},
                                          new[] {5, 2},
                                          new[] {5, 1},
                                          new[] {5, 0}
                                      };
            positionsToBuildAround3 = new List<int[]>
                                      {
                                          new[] {-1, -1},
                                          new[] {0, -1},
                                          new[] {1, -1},
                                          new[] {2, -1},
                                          new[] {3, -1},

                                          new[] {3, 0},
                                          new[] {3, 1},
                                          new[] {3, 2},
                                          new[] {3, 3},

                                          new[] {2, 3},
                                          new[] {1, 3},
                                          new[] {0, 3},
                                          new[] {-1, 3},

                                          new[] {-1, 2},
                                          new[] {-1, 1},
                                          new[] {-1, 0}
                                      };
            positionsToBuildAround2 = new List<int[]>
                                      {
                                          new[] {-1, -1},
                                          new[] {-1, 0},
                                          new[] {-1, 1},
                                          new[] {-1, 2},
                                          new[] {0, 2},
                                          new[] {1, 2},
                                          new[] {2, 2},
                                          new[] {2, 1},
                                          new[] {2, 0},
                                          new[] {2, -1},
                                          new[] {1, -1},
                                          new[] {0, -1},
                                      };
        }

        public Action GetAction(PlayerView playerView, DebugInterface debugInterface)
        {
            actions = new Dictionary<int, EntityAction>();
            View = playerView;
            Debug = debugInterface;

            Around.Scan(View);

            CommandBuildingsWorkers();
            CommandBuildingsRange();

            CommandUnitsWorkers();
            CommandUnitsRanged();
            CommandUnitsMelee();
            CommandUnitsTurrets();

            return new Action(actions);
        }

        #region Buildings

        private void CommandBuildingsWorkers()
        {
            var workerUnitsCount = Around.PopulationProvide * Around.unitsRatio[0];

            foreach (var builderBuilding in Around.MyBuildingsWorkers)
            {
                var needBuildBuilders = Around.MyUnitsWorkers.Count() < workerUnitsCount
                                        && Around.Me.Resource >= Around.WorkerUnitCost;

                actions.Add(builderBuilding.Id,
                            needBuildBuilders
                                ? new EntityAction(null, new BuildAction(EntityType.BuilderUnit, GetRndPositionAround(builderBuilding)), null, null)
                                : new EntityAction(null, null, null, null));
            }
        }

        private void CommandBuildingsRange()
        {
            var rangeUnitsCount = Around.PopulationProvide * Around.unitsRatio[1];

            foreach (var rangeBuilding in Around.MyBuildingsRanged)
            {
                var needBuildRanged = Around.MyUnitsRanged.Count() < rangeUnitsCount
                                      && Around.Me.Resource >= Around.RangedUnitCost;

                actions.Add(rangeBuilding.Id,
                            needBuildRanged
                                ? new EntityAction(null, new BuildAction(EntityType.RangedUnit, GetRndPositionAround(rangeBuilding)), null, null)
                                : new EntityAction(null, null, null, null));
            }
        }

        private Vec2Int GetRndPositionAround(Entity building) // recursively delete
        {
            var buildingSize = View.EntityProperties.Single(ep => ep.Key == building.EntityType).Value.Size;

            List<int[]> targetCollection;

            if (buildingSize == 5)
            {
                targetCollection = positionsToBuildAround5;
            }
            else if (buildingSize == 3)
            {
                targetCollection = positionsToBuildAround3;
            }
            else
            {
                targetCollection = positionsToBuildAround2;
            }

            var rndPositionToBuild = targetCollection.ElementAt(new Random().Next(targetCollection.Count));

            var positionToBuild = new Vec2Int(building.Position.X + rndPositionToBuild.ElementAt(0),
                                              building.Position.Y + rndPositionToBuild.ElementAt(1));

            // if (recursivelyTillFind)
            // {
            //     return Around.MyUnits.Any(my => my.Position.X == positionToBuild.X &&
            //                                     my.Position.Y == positionToBuild.Y)
            //                ? GetRndPositionAround(building)
            //                : positionToBuild;
            // }

            return positionToBuild;
        }

        #endregion

        #region Units

        private void CommandUnitsWorkers()
        {
            foreach (var builderUnit in Around.MyUnitsWorkers) // All goes get spice milange because SPICE MUST FLOW
            {
                var nearestNotBusySpice = Around.GetNearestNotBusySpice(builderUnit.Position);

                var moveAction = new MoveAction(nearestNotBusySpice.Position, true, false);
                var attackAction = new AttackAction(nearestNotBusySpice.Id, null);

                var action = new EntityAction(moveAction, null, attackAction, null);

                actions.Add(builderUnit.Id, action);
            }

            if (Around.RepairNeeds)
            {
                foreach (var brokenBuilding in Around.MyBuildingsBroken)
                {
                    var nearestBuilder = Around.GetNearestWorker(brokenBuilding.Position);
                    actions.Remove(nearestBuilder.Id);

                    var moveAction = new MoveAction(brokenBuilding.Position, true, false);
                    var repairAction = new RepairAction(brokenBuilding.Id);
                    actions.Add(nearestBuilder.Id, new EntityAction(moveAction, null, null, repairAction));

                    // var hasBigDamage = brokenBuilding.Health <= View.EntityProperties.Single(ep => ep.Key == brokenBuilding.EntityType).Value.MaxHealth / 2;
                    // if (hasBigDamage)
                    // {
                    //     var nearestBuilder2 = Around.GetNearestWorker(brokenBuilding.Position);
                    //     actions.Remove(nearestBuilder2.Id);
                    //
                    //     moveAction = new MoveAction(brokenBuilding.Position, true, false);
                    //     repairAction = new RepairAction(brokenBuilding.Id);
                    //     actions.Add(nearestBuilder2.Id, new EntityAction(moveAction, null, null, repairAction));
                    // }
                }
            }

            if (Around.NeedBuildBuildingWorkers)
            {
                SendWorkerToBuild(EntityType.BuilderBase);
            }
            else if (Around.NeedBuildBuildingRanged)
            {
                SendWorkerToBuild(EntityType.RangedBase);
            }
            else if (Around.NeedBuildHouse)
            {
                SendWorkerToBuild(EntityType.House);
            }
        }

        private void SendWorkerToBuild(EntityType type)
        {
            var buildingSize = View.EntityProperties.Single(ep => ep.Key == type).Value.Size;
            var workersWhoCanBuildRightHereRightNow = Around.MyUnitsWorkers.Where(w => w.GetMappingAround(buildingSize).HasPlaceToBuildAround(Around, buildingSize)).ToList();
            if (workersWhoCanBuildRightHereRightNow.Any()) // once can build right here right now
            {
                var workerWhoCanBuild = workersWhoCanBuildRightHereRightNow.First();
                var mappingAround = workerWhoCanBuild.GetMappingAround(buildingSize);
                var pointToBuild = mappingAround.GetFirstPointToBuildAround(workerWhoCanBuild, Around, buildingSize);
                // else // no who can build right here right now, need to find nearest place
                // {
                //     pointToBuild = Around.FreePoints.GetFirstPointToBuildNear(Around, buildingSize);
                //     workerWhoCanBuild = Around.GetNearestEntityOfType(pointToBuild, PlayerType.My, EntityType.BuilderUnit);
                // }

                actions.Remove(workerWhoCanBuild.Id);
                actions.Add(workerWhoCanBuild.Id, new EntityAction(null, new BuildAction(type, pointToBuild), null, null));
            }
        }

        private void CommandUnitsRanged()
        {
            foreach (var rangedUnit in Around.MyUnitsRanged)
            {
                var moveAction = new MoveAction();
                var attackAction = new AttackAction();
                var action = new EntityAction();

                if (Around.Behavior == BehaviorType.Aggressive)
                {
                    var nearestEnemy = Around.GetNearestEntity(rangedUnit.Position, PlayerType.Enemy);
                    moveAction = new MoveAction(nearestEnemy.Position, true, false);
                    attackAction = new AttackAction(nearestEnemy.Id, null);
                }
                else
                {
                    // var topBuilding = Around.MyBuildings
                    //                         .OrderByDescending(b => b.Position.X + b.Position.Y)
                    //                         .FirstOrDefault();
                    // moveAction = new MoveAction(GetRndPositionAround(topBuilding, false), true, false);
                    moveAction = new MoveAction(rangedUnit.Position, true, false);
                }

                action = new EntityAction(moveAction, null, attackAction, null);

                actions.Add(rangedUnit.Id, action);
            }
        }

        private void CommandUnitsMelee()
        {
            foreach (var meleeUnit in Around.MyUnitsMelees)
            {
                var moveAction = new MoveAction();
                var attackAction = new AttackAction();
                var action = new EntityAction();

                if (Around.Behavior == BehaviorType.Aggressive)
                {
                    var nearestEnemy = Around.GetNearestEntity(meleeUnit.Position, PlayerType.Enemy);
                    moveAction = new MoveAction(nearestEnemy.Position, true, false);
                    attackAction = new AttackAction(nearestEnemy.Id, null);
                }
                else
                {
                    // var topBuilding = Around.MyBuildings
                    //                         .OrderByDescending(b => b.Position.X + b.Position.Y)
                    //                         .FirstOrDefault();
                    // moveAction = new MoveAction(GetRndPositionAround(topBuilding, false), true, false);
                    moveAction = new MoveAction(meleeUnit.Position, true, false);
                }

                action = new EntityAction(moveAction, null, attackAction, null);

                actions.Add(meleeUnit.Id, action);
            }
        }

        private void CommandUnitsTurrets()
        {
            foreach (var turretUnit in Around.MyUnitsTurrets)
            {
                var nearestEnemy = Around.GetNearestEntity(turretUnit.Position, PlayerType.Enemy);

                var attackAction = new AttackAction(nearestEnemy.Id, null);

                var action = new EntityAction(null, null, attackAction, null);

                actions.Add(turretUnit.Id, action);
            }
        }

        #endregion

        public void DebugUpdate(PlayerView playerView, DebugInterface debugInterface)
        {
            debugInterface.Send(new DebugCommand.Clear());
            debugInterface.GetState();
        }
    }
}