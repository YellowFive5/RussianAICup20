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

        public MyStrategy()
        {
            Around = new World();
        }

        public Action GetAction(PlayerView playerView, DebugInterface debugInterface)
        {
            actions = new Dictionary<int, EntityAction>();
            View = playerView;
            Debug = debugInterface;

            Around.Scan(View);
            Around.ChooseBehavior();

            CommandBuildingsWorkers();
            CommandBuildingsRange();

            CommandUnitsWorkers();
            CommandUnitsRanged();
            CommandUnitsMelee();
            CommandUnitsTurrets();

            return new Action(actions);
        }

        private void CommandBuildingsWorkers()
        {
            var workerUnitsCount = Around.Behavior == BehaviorType.Passive
                                       ? (int) (Around.PopulationProvide * 0.8)
                                       : (int) (Around.PopulationProvide * 0.2);

            var lstX = new List<int> {-1, 5};

            foreach (var builderBuilding in Around.MyBuildingsWorkers)
            {
                var positionToBuild = new Vec2Int(builderBuilding.Position.X + lstX.ElementAt(new Random().Next(lstX.Count)),
                                                  builderBuilding.Position.Y + new Random().Next(0, 5));

                var needBuildBuilders = Around.MyUnitsWorkers.Count() < workerUnitsCount
                                        && Around.Me.Resource >= Around.WorkerUnitCost;

                actions.Add(builderBuilding.Id,
                            needBuildBuilders
                                ? new EntityAction(null, new BuildAction(EntityType.BuilderUnit, positionToBuild), null, null)
                                : new EntityAction(null, null, null, null));
            }
        }

        private void CommandBuildingsRange()
        {
            var rangeUnitsCount = Around.Behavior == BehaviorType.Passive
                                      ? (int) (Around.PopulationProvide * 0.2)
                                      : (int) (Around.PopulationProvide * 0.8);

            var lstX = new List<int> {-1, 5};

            foreach (var rangeBuilding in Around.MyBuildingsRanged)
            {
                var positionToBuild = new Vec2Int(rangeBuilding.Position.X + lstX.ElementAt(new Random().Next(lstX.Count)),
                                                  rangeBuilding.Position.Y + new Random().Next(0, 5));

                var needBuildRanged = Around.MyUnitsRanged.Count() < rangeUnitsCount
                                      && Around.Me.Resource >= Around.RangedUnitCost;

                actions.Add(rangeBuilding.Id,
                            needBuildRanged
                                ? new EntityAction(null, new BuildAction(EntityType.RangedUnit, positionToBuild), null, null)
                                : new EntityAction(null, null, null, null));
            }
        }

        private void CommandUnitsWorkers()
        {
            foreach (var builderUnit in Around.MyUnitsWorkers) // Go get spice milange
            {
                var nearestSpice = Around.GetNearestEntityOfType(builderUnit, PlayerType.My, EntityType.Resource);

                var moveAction = new MoveAction(nearestSpice.Position, true, false);
                var attackAction = new AttackAction(nearestSpice.Id, null);
                var action = new EntityAction(moveAction, null, attackAction, null);

                actions.Add(builderUnit.Id, action);
            }

            if (Around.NeedBuildHouse)
            {
                SendNearestWorkerToBuild(EntityType.House);
            }

            foreach (var brokenBuilding in Around.MyBuildingsBroken) // Repair needs
            {
                var nearestBuilder = Around.GetNearestEntityOfType(brokenBuilding, PlayerType.My, EntityType.BuilderUnit);
                actions.Remove(nearestBuilder.Id);

                var moveAction = new MoveAction(brokenBuilding.Position, true, false);
                var repairAction = new RepairAction(brokenBuilding.Id);
                actions.Add(nearestBuilder.Id, new EntityAction(moveAction, null, null, repairAction));
            }

            if (Around.NeedBuildBuildingRanged)
            {
                SendNearestWorkerToBuild(EntityType.RangedBase);
            }

            if (Around.NeedBuildBuildingWorkers)
            {
                SendNearestWorkerToBuild(EntityType.BuilderBase);
            }
        }

        private void SendNearestWorkerToBuild(EntityType type)
        {
            var buildingSize = View.EntityProperties.Single(ep => ep.Key == type).Value.Size;
            var workersWhoCanBuild = Around.MyUnitsWorkers.Where(w => // Find worker who can build right now and here
                                                                 {
                                                                     var _1 = new List<Vec2Int>();
                                                                     for (var x = 0; x < buildingSize; x++)
                                                                     {
                                                                         for (var y = 0; y < buildingSize; y++)
                                                                         {
                                                                             var _x = w.Position.X + x;
                                                                             var _y = w.Position.Y + y + 1;
                                                                             if (_x >= 0 && _y >= 0 && _x < 80 && _y < 80)
                                                                             {
                                                                                 _1.Add(new Vec2Int(_x, _y));
                                                                             }
                                                                         }
                                                                     }

                                                                     var _2 = new List<Vec2Int>();
                                                                     for (var x = 0; x < buildingSize; x++)
                                                                     {
                                                                         for (var y = 0; y < buildingSize; y++)
                                                                         {
                                                                             var _x = w.Position.X + x + 1;
                                                                             var _y = w.Position.Y + y;
                                                                             if (_x >= 0 && _y >= 0 && _x < 80 && _y < 80)
                                                                             {
                                                                                 _2.Add(new Vec2Int(_x, _y));
                                                                             }
                                                                         }
                                                                     }

                                                                     var _3 = new List<Vec2Int>();
                                                                     for (var x = 0; x < buildingSize; x++)
                                                                     {
                                                                         for (var y = 0; y < buildingSize; y++)
                                                                         {
                                                                             var _x = w.Position.X + x + 1;
                                                                             var _y = w.Position.Y + y + 1 - buildingSize;
                                                                             if (_x >= 0 && _y >= 0 && _x < 80 && _y < 80)
                                                                             {
                                                                                 _3.Add(new Vec2Int(_x, _y));
                                                                             }
                                                                         }
                                                                     }

                                                                     var _4 = new List<Vec2Int>();
                                                                     for (var x = 0; x < buildingSize; x++)
                                                                     {
                                                                         for (var y = 0; y < buildingSize; y++)
                                                                         {
                                                                             var _x = w.Position.X + x;
                                                                             var _y = w.Position.Y + y - buildingSize;
                                                                             if (_x >= 0 && _y >= 0 && _x < 80 && _y < 80)
                                                                             {
                                                                                 _4.Add(new Vec2Int(_x, _y));
                                                                             }
                                                                         }
                                                                     }

                                                                     var _5 = new List<Vec2Int>();
                                                                     for (var x = 0; x < buildingSize; x++)
                                                                     {
                                                                         for (var y = 0; y < buildingSize; y++)
                                                                         {
                                                                             var _x = w.Position.X + x + 1 - buildingSize;
                                                                             var _y = w.Position.Y + y - buildingSize;
                                                                             if (_x >= 0 && _y >= 0 && _x < 80 && _y < 80)
                                                                             {
                                                                                 _5.Add(new Vec2Int(_x, _y));
                                                                             }
                                                                         }
                                                                     }

                                                                     var _6 = new List<Vec2Int>();
                                                                     for (var x = 0; x < buildingSize; x++)
                                                                     {
                                                                         for (var y = 0; y < buildingSize; y++)
                                                                         {
                                                                             var _x = w.Position.X + x - buildingSize;
                                                                             var _y = w.Position.Y + y + 1 - buildingSize;
                                                                             if (_x >= 0 && _y >= 0 && _x < 80 && _y < 80)
                                                                             {
                                                                                 _6.Add(new Vec2Int(_x, _y));
                                                                             }
                                                                         }
                                                                     }

                                                                     var _7 = new List<Vec2Int>();
                                                                     for (var x = 0; x < buildingSize; x++)
                                                                     {
                                                                         for (var y = 0; y < buildingSize; y++)
                                                                         {
                                                                             var _x = w.Position.X + x - buildingSize;
                                                                             var _y = w.Position.Y + y;
                                                                             if (_x >= 0 && _y >= 0 && _x < 80 && _y < 80)
                                                                             {
                                                                                 _7.Add(new Vec2Int(_x, _y));
                                                                             }
                                                                         }
                                                                     }

                                                                     var _8 = new List<Vec2Int>();
                                                                     for (var x = 0; x < buildingSize; x++)
                                                                     {
                                                                         for (var y = 0; y < buildingSize; y++)
                                                                         {
                                                                             var _x = w.Position.X + x + 1 - buildingSize;
                                                                             var _y = w.Position.Y + y + 1;
                                                                             if (_x >= 0 && _y >= 0 && _x < 80 && _y < 80)
                                                                             {
                                                                                 _8.Add(new Vec2Int(_x, _y));
                                                                             }
                                                                         }
                                                                     }

                                                                     var points = new List<List<Vec2Int>>
                                                                                  {
                                                                                      _1,
                                                                                      _2,
                                                                                      _3,
                                                                                      _4,
                                                                                      _5,
                                                                                      _6,
                                                                                      _7,
                                                                                      _8,
                                                                                  };

                                                                     return points.Any(l => l.Count == buildingSize * buildingSize && !l.Any(s => Around.NotFreeSpace.Any(np => np.X == s.X && np.Y == s.Y)));
                                                                 }).Take(1).ToList();

            if (!workersWhoCanBuild.Any())
            {
                return;
            }

            var workerWhoCanBuild = workersWhoCanBuild.First();

            var pointToBuild = new Vec2Int();

            var _1 = new List<Vec2Int>();
            for (var x = 0; x < buildingSize; x++)
            {
                for (var y = 0; y < buildingSize; y++)
                {
                    var _x = workerWhoCanBuild.Position.X + x;
                    var _y = workerWhoCanBuild.Position.Y + y + 1;
                    if (_x >= 0 && _y >= 0 && _x < 80 && _y < 80)
                    {
                        _1.Add(new Vec2Int(_x, _y));
                    }
                }
            }

            var _2 = new List<Vec2Int>();
            for (var x = 0; x < buildingSize; x++)
            {
                for (var y = 0; y < buildingSize; y++)
                {
                    var _x = workerWhoCanBuild.Position.X + x + 1;
                    var _y = workerWhoCanBuild.Position.Y + y;
                    if (_x >= 0 && _y >= 0 && _x < 80 && _y < 80)
                    {
                        _2.Add(new Vec2Int(_x, _y));
                    }
                }
            }

            var _3 = new List<Vec2Int>();
            for (var x = 0; x < buildingSize; x++)
            {
                for (var y = 0; y < buildingSize; y++)
                {
                    var _x = workerWhoCanBuild.Position.X + x + 1;
                    var _y = workerWhoCanBuild.Position.Y + y + 1 - buildingSize;
                    if (_x >= 0 && _y >= 0 && _x < 80 && _y < 80)
                    {
                        _3.Add(new Vec2Int(_x, _y));
                    }
                }
            }

            var _4 = new List<Vec2Int>();
            for (var x = 0; x < buildingSize; x++)
            {
                for (var y = 0; y < buildingSize; y++)
                {
                    var _x = workerWhoCanBuild.Position.X + x;
                    var _y = workerWhoCanBuild.Position.Y + y - buildingSize;
                    if (_x >= 0 && _y >= 0 && _x < 80 && _y < 80)
                    {
                        _4.Add(new Vec2Int(_x, _y));
                    }
                }
            }

            var _5 = new List<Vec2Int>();
            for (var x = 0; x < buildingSize; x++)
            {
                for (var y = 0; y < buildingSize; y++)
                {
                    var _x = workerWhoCanBuild.Position.X + x + 1 - buildingSize;
                    var _y = workerWhoCanBuild.Position.Y + y - buildingSize;
                    if (_x >= 0 && _y >= 0 && _x < 80 && _y < 80)
                    {
                        _5.Add(new Vec2Int(_x, _y));
                    }
                }
            }

            var _6 = new List<Vec2Int>();
            for (var x = 0; x < buildingSize; x++)
            {
                for (var y = 0; y < buildingSize; y++)
                {
                    var _x = workerWhoCanBuild.Position.X + x - buildingSize;
                    var _y = workerWhoCanBuild.Position.Y + y + 1 - buildingSize;
                    if (_x >= 0 && _y >= 0 && _x < 80 && _y < 80)
                    {
                        _6.Add(new Vec2Int(_x, _y));
                    }
                }
            }

            var _7 = new List<Vec2Int>();
            for (var x = 0; x < buildingSize; x++)
            {
                for (var y = 0; y < buildingSize; y++)
                {
                    var _x = workerWhoCanBuild.Position.X + x - buildingSize;
                    var _y = workerWhoCanBuild.Position.Y + y;
                    if (_x >= 0 && _y >= 0 && _x < 80 && _y < 80)
                    {
                        _7.Add(new Vec2Int(_x, _y));
                    }
                }
            }

            var _8 = new List<Vec2Int>();
            for (var x = 0; x < buildingSize; x++)
            {
                for (var y = 0; y < buildingSize; y++)
                {
                    var _x = workerWhoCanBuild.Position.X + x + 1 - buildingSize;
                    var _y = workerWhoCanBuild.Position.Y + y + 1;
                    if (_x >= 0 && _y >= 0 && _x < 80 && _y < 80)
                    {
                        _8.Add(new Vec2Int(_x, _y));
                    }
                }
            }

            if (_1.Count.Equals(buildingSize * buildingSize) && !_1.Any(s => Around.NotFreeSpace.Any(np => np.X == s.X && np.Y == s.Y)))
            {
                pointToBuild = new Vec2Int(workerWhoCanBuild.Position.X, workerWhoCanBuild.Position.Y + 1);
            }
            else if (_2.Count.Equals(buildingSize * buildingSize) && !_2.Any(s => Around.NotFreeSpace.Any(np => np.X == s.X && np.Y == s.Y)))
            {
                pointToBuild = new Vec2Int(workerWhoCanBuild.Position.X + 1, workerWhoCanBuild.Position.Y);
            }
            else if (_3.Count.Equals(buildingSize * buildingSize) && !_3.Any(s => Around.NotFreeSpace.Any(np => np.X == s.X && np.Y == s.Y)))
            {
                pointToBuild = new Vec2Int(workerWhoCanBuild.Position.X + 1, workerWhoCanBuild.Position.Y + 1 - buildingSize);
            }
            else if (_4.Count.Equals(buildingSize * buildingSize) && !_4.Any(s => Around.NotFreeSpace.Any(np => np.X == s.X && np.Y == s.Y)))
            {
                pointToBuild = new Vec2Int(workerWhoCanBuild.Position.X, workerWhoCanBuild.Position.Y - buildingSize);
            }
            else if (_5.Count.Equals(buildingSize * buildingSize) && !_5.Any(s => Around.NotFreeSpace.Any(np => np.X == s.X && np.Y == s.Y)))
            {
                pointToBuild = new Vec2Int(workerWhoCanBuild.Position.X + 1 - buildingSize, workerWhoCanBuild.Position.Y - buildingSize);
            }
            else if (_6.Count.Equals(buildingSize * buildingSize) && !_6.Any(s => Around.NotFreeSpace.Any(np => np.X == s.X && np.Y == s.Y)))
            {
                pointToBuild = new Vec2Int(workerWhoCanBuild.Position.X - buildingSize, workerWhoCanBuild.Position.Y - buildingSize + 1);
            }
            else if (_7.Count.Equals(buildingSize * buildingSize) && !_7.Any(s => Around.NotFreeSpace.Any(np => np.X == s.X && np.Y == s.Y)))
            {
                pointToBuild = new Vec2Int(workerWhoCanBuild.Position.X - buildingSize, workerWhoCanBuild.Position.Y);
            }
            else if (_8.Count.Equals(buildingSize * buildingSize) && !_8.Any(s => Around.NotFreeSpace.Any(np => np.X == s.X && np.Y == s.Y)))
            {
                pointToBuild = new Vec2Int(workerWhoCanBuild.Position.X + 1 - buildingSize, workerWhoCanBuild.Position.Y + 1);
            }

            if (pointToBuild.X >= 0 && pointToBuild.Y >= 0 && pointToBuild.X < 80 && pointToBuild.Y < 80)
            {
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
                    var nearestEnemy = Around.GetNearestEntity(rangedUnit, PlayerType.Enemy);
                    moveAction = new MoveAction(nearestEnemy.Position, true, false);
                    attackAction = new AttackAction(nearestEnemy.Id, null);
                }
                else
                {
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
                    var nearestEnemy = Around.GetNearestEntity(meleeUnit, PlayerType.Enemy);
                    moveAction = new MoveAction(nearestEnemy.Position, true, false);
                    attackAction = new AttackAction(nearestEnemy.Id, null);
                }
                else
                {
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
                var nearestEnemy = Around.GetNearestEntity(turretUnit, PlayerType.Enemy);

                var attackAction = new AttackAction(nearestEnemy.Id, null);

                var action = new EntityAction(null, null, attackAction, null);

                actions.Add(turretUnit.Id, action);
            }
        }

        public void DebugUpdate(PlayerView playerView, DebugInterface debugInterface)
        {
            debugInterface.Send(new DebugCommand.Clear());
            debugInterface.GetState();
        }
    }
}