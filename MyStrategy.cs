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
            var lstXY = new List<int> {-1, 1};

            foreach (var builderUnit in Around.MyUnitsWorkers)
            {
                if (Around.NeedRepairHouses)
                {
                    var buildingToRepair = Around.MyBuildingsBroken.First();
                    var moveAction = new MoveAction(buildingToRepair.Position, true, false);
                    var repairAction = new RepairAction(buildingToRepair.Id);
                    actions.Add(builderUnit.Id, new EntityAction(moveAction, null, null, repairAction));
                }
                else if (Around.NeedBuildHouse && Around.CanBuildHouse)
                {
                    var position = new Vec2Int(builderUnit.Position.X + lstXY.ElementAt(new Random().Next(lstXY.Count)),
                                               builderUnit.Position.Y + lstXY.ElementAt(new Random().Next(lstXY.Count)));
                    actions.Add(builderUnit.Id, new EntityAction(null, new BuildAction(EntityType.House, position), null, null));
                }
                else
                {
                    var nearestSpice = Around.GetNearestEntityOfType(builderUnit, PlayerType.My, EntityType.Resource);

                    var moveAction = new MoveAction(nearestSpice.Position, true, false);
                    var attackAction = new AttackAction(nearestSpice.Id, null);
                    var action = new EntityAction(moveAction, null, attackAction, null);

                    actions.Add(builderUnit.Id, action);
                }
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