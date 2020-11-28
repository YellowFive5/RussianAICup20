#region Usings

using System.Collections.Generic;
using System.Linq;
using Aicup2020.Model;

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
            var workerUnitsCount = 7;

            foreach (var builderBuilding in Around.MyBuildingsWorkers)
            {
                var positionToBuild = new Vec2Int(builderBuilding.Position.X + 5, builderBuilding.Position.Y);

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
            var rangeUnitsCount = 8;

            foreach (var rangeBuilding in Around.MyBuildingsRanged)
            {
                var positionToBuild = new Vec2Int(rangeBuilding.Position.X + 5, rangeBuilding.Position.Y);

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
            // var needBuildHouse = Around.PopulationFree <= 0; // todo house builds
            // var canBuildHouse = Around.Me.Resource >= Around.HouseBuildingCost;
            //
            // if (needBuildHouse && canBuildHouse)
            // {
            //     var position = new Vec2Int(builderBuilding.Position.X + 10, builderBuilding.Position.Y + 10);
            //     Actions.Add(builderBuilding.Id, new EntityAction(null, new BuildAction(EntityType.House, position), null, null));
            // }

            foreach (var builderUnit in Around.MyUnitsWorkers)
            {
                var nearestSpice = Around.GetNearestTo(builderUnit, PlayerType.My, EntityType.Resource);

                var moveAction = new MoveAction(nearestSpice.Position, true, false);
                var attackAction = new AttackAction(nearestSpice.Id, null);

                var action = new EntityAction(moveAction, null, attackAction, null);

                actions.Add(builderUnit.Id, action);
            }
        }

        private void CommandUnitsRanged()
        {
            foreach (var rangedUnit in Around.MyUnitsRanged)
            {
                var nearestEnemy = Around.GetNearestTo(rangedUnit, PlayerType.Enemy, GetEntityToAttack());

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
                var nearestEnemy = Around.GetNearestTo(meleeUnit, PlayerType.Enemy, GetEntityToAttack());

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