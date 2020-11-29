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
            var workerUnitsCount = (int) (Around.PopulationProvide * 0.4);

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
            var rangeUnitsCount = (int) (Around.PopulationProvide * 0.6);

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

            if (Around.NeedBuildHouse && Around.CanBuildHouse) // New houses needs
            {
                var workerToBuildHouse = Around.MyUnitsWorkers.Where(w => // Find worker who can build right now and here
                                                                      {
                                                                          var leftTop = new List<Vec2Int>();
                                                                          for (var x = 0; x < 3; x++)
                                                                          {
                                                                              for (var y = 0; y < 3; y++)
                                                                              {
                                                                                  leftTop.Add(new Vec2Int(w.Position.X + x, w.Position.Y + y + 1));
                                                                              }
                                                                          }

                                                                          var rightTop = new List<Vec2Int>();
                                                                          for (var x = 0; x < 3; x++)
                                                                          {
                                                                              for (var y = 0; y < 3; y++)
                                                                              {
                                                                                  rightTop.Add(new Vec2Int(w.Position.X + x + 1, w.Position.Y - y + 1));
                                                                              }
                                                                          }

                                                                          var rightBottom = new List<Vec2Int>();
                                                                          for (var x = 0; x < 3; x++)
                                                                          {
                                                                              for (var y = 0; y < 3; y++)
                                                                              {
                                                                                  rightBottom.Add(new Vec2Int(w.Position.X - x + 1, w.Position.Y - y));
                                                                              }
                                                                          }

                                                                          var leftBottom = new List<Vec2Int>();
                                                                          for (var x = 0; x < 3; x++)
                                                                          {
                                                                              for (var y = 0; y < 3; y++)
                                                                              {
                                                                                  leftBottom.Add(new Vec2Int(w.Position.X - x, w.Position.Y + y));
                                                                              }
                                                                          }

                                                                          return !leftTop.Any(s => Around.NotFreeSpace.Any(np => np.X == s.X && np.Y == s.Y)) && !rightTop.Any(s => Around.NotFreeSpace.Any(np => np.X == s.X && np.Y == s.Y)) ||
                                                                                 !rightTop.Any(s => Around.NotFreeSpace.Any(np => np.X == s.X && np.Y == s.Y)) && !rightBottom.Any(s => Around.NotFreeSpace.Any(np => np.X == s.X && np.Y == s.Y)) ||
                                                                                 !rightBottom.Any(s => Around.NotFreeSpace.Any(np => np.X == s.X && np.Y == s.Y)) && !leftBottom.Any(s => Around.NotFreeSpace.Any(np => np.X == s.X && np.Y == s.Y)) ||
                                                                                 !leftBottom.Any(s => Around.NotFreeSpace.Any(np => np.X == s.X && np.Y == s.Y)) && !leftTop.Any(s => Around.NotFreeSpace.Any(np => np.X == s.X && np.Y == s.Y));
                                                                      }).FirstOrDefault();
                var pointToBuild = new Vec2Int();
                var leftTop = new List<Vec2Int>();
                for (var x = 0; x < 3; x++)
                {
                    for (var y = 0; y < 3; y++)
                    {
                        leftTop.Add(new Vec2Int(workerToBuildHouse.Position.X + x, workerToBuildHouse.Position.Y + y + 1));
                    }
                }
                var rightTop = new List<Vec2Int>();
                for (var x = 0; x < 3; x++)
                {
                    for (var y = 0; y < 3; y++)
                    {
                        rightTop.Add(new Vec2Int(workerToBuildHouse.Position.X + x + 1, workerToBuildHouse.Position.Y - y + 1));
                    }
                }
                var rightBottom = new List<Vec2Int>();
                for (var x = 0; x < 3; x++)
                {
                    for (var y = 0; y < 3; y++)
                    {
                        rightBottom.Add(new Vec2Int(workerToBuildHouse.Position.X - x + 1, workerToBuildHouse.Position.Y - y));
                    }
                }
                var leftBottom = new List<Vec2Int>();
                for (var x = 0; x < 3; x++)
                {
                    for (var y = 0; y < 3; y++)
                    {
                        leftBottom.Add(new Vec2Int(workerToBuildHouse.Position.X - x, workerToBuildHouse.Position.Y + y));
                    }
                }
                if (!rightTop.Any(s => Around.NotFreeSpace.Any(np => np.X == s.X && np.Y == s.Y)))
                {
                    pointToBuild = new Vec2Int(workerToBuildHouse.Position.X, workerToBuildHouse.Position.Y + 1);
                }
                else if (!leftTop.Any(s => Around.NotFreeSpace.Any(np => np.X == s.X && np.Y == s.Y)))
                {
                    pointToBuild = new Vec2Int(workerToBuildHouse.Position.X - 3, workerToBuildHouse.Position.Y);
                }
                else if (!rightBottom.Any(s => Around.NotFreeSpace.Any(np => np.X == s.X && np.Y == s.Y)))
                {
                    pointToBuild = new Vec2Int(workerToBuildHouse.Position.X + 1, workerToBuildHouse.Position.Y);
                }
                else if (!leftBottom.Any(s => Around.NotFreeSpace.Any(np => np.X == s.X && np.Y == s.Y)))
                {
                    pointToBuild = new Vec2Int(workerToBuildHouse.Position.X - 3, workerToBuildHouse.Position.Y - 1);
                }

                actions.Remove(workerToBuildHouse.Id);
                actions.Add(workerToBuildHouse.Id, new EntityAction(null, new BuildAction(EntityType.House, pointToBuild), null, null));
            }

            foreach (var brokenBuilding in Around.MyBuildingsBroken) // Repair needs
            {
                var nearestBuilder = Around.GetNearestEntityOfType(brokenBuilding, PlayerType.My, EntityType.BuilderUnit);
                actions.Remove(nearestBuilder.Id);

                var moveAction = new MoveAction(brokenBuilding.Position, true, false);
                var repairAction = new RepairAction(brokenBuilding.Id);
                actions.Add(nearestBuilder.Id, new EntityAction(moveAction, null, null, repairAction));
            }
        }

        private void CommandUnitsRanged()
        {
            foreach (var rangedUnit in Around.MyUnitsRanged)
            {
                var nearestEnemy = Around.GetNearestEntity(rangedUnit, PlayerType.Enemy);

                var moveAction = new MoveAction(nearestEnemy.Position, true, true);
                var attackAction = new AttackAction(nearestEnemy.Id, null);

                var action = new EntityAction(moveAction, null, attackAction, null);

                actions.Add(rangedUnit.Id, action);
            }
        }

        private void CommandUnitsMelee()
        {
            foreach (var meleeUnit in Around.MyUnitsMelees)
            {
                var nearestEnemy = Around.GetNearestEntity(meleeUnit, PlayerType.Enemy);

                var moveAction = new MoveAction(nearestEnemy.Position, true, true);
                var attackAction = new AttackAction(nearestEnemy.Id, null);

                var action = new EntityAction(moveAction, null, attackAction, null);

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

        private EntityType GetEntityToAttack()
        {
            if (Around.EnemyUnitsRanged.Any())
            {
                return EntityType.RangedUnit;
            }

            if (Around.EnemyUnitsMelees.Any())
            {
                return EntityType.MeleeUnit;
            }

            if (Around.EnemyUnitsTurrets.Any())
            {
                return EntityType.Turret;
            }

            if (Around.EnemyUnitsWorkers.Any())
            {
                return EntityType.BuilderUnit;
            }

            if (Around.EnemyBuildingsRanged.Any())
            {
                return EntityType.RangedBase;
            }

            if (Around.EnemyBuildingsMelees.Any())
            {
                return EntityType.MeleeBase;
            }

            if (Around.EnemyBuildingsWorkers.Any())
            {
                return EntityType.BuilderBase;
            }

            if (Around.EnemyBuildingsHouses.Any())
            {
                return EntityType.House;
            }

            return EntityType.Wall;
        }

        public void DebugUpdate(PlayerView playerView, DebugInterface debugInterface)
        {
            debugInterface.Send(new DebugCommand.Clear());
            debugInterface.GetState();
        }
    }
}