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
        private Dictionary<int, EntityAction> Actions;

        public MyStrategy()
        {
            Around = new World();
        }

        public Action GetAction(PlayerView playerView, DebugInterface debugInterface)
        {
            Actions = new Dictionary<int, EntityAction>();
            View = playerView;
            Debug = debugInterface;

            Around.Scan(View);

            CommandBuildingsBuilders();
            CommandRangeBuilders();

            CommandUnitsBuilders();
            CommandUnitsRanged();
            CommandUnitsMelee();

            return new Action(Actions);
        }

        private void CommandBuildingsBuilders()
        {
            var builderUnitsCount = 7;

            foreach (var builderBuilding in Around.MyBuildingsBuilders)
            {
                var positionToBuild = new Vec2Int(builderBuilding.Position.X + 5, builderBuilding.Position.Y);

                var needBuildBuilders = Around.MyUnitsBuilders.Count() < builderUnitsCount
                                        && Around.Me.Resource >= Around.BuilderUnitCost;

                if (needBuildBuilders)
                {
                    Actions.Add(builderBuilding.Id, new EntityAction(null, new BuildAction(EntityType.BuilderUnit, positionToBuild), null, null));
                }
            }
        }

        private void CommandRangeBuilders()
        {
            var rangeUnitsCount = 8;

            foreach (var rangeBuilding in Around.MyBuildingsRanged)
            {
                var positionToBuild = new Vec2Int(rangeBuilding.Position.X + 5, rangeBuilding.Position.Y);

                var needBuildRanged = Around.MyUnitsRanged.Count() < rangeUnitsCount
                                      && Around.Me.Resource >= Around.RangedUnitCost;

                if (needBuildRanged)
                {
                    Actions.Add(rangeBuilding.Id, new EntityAction(null, new BuildAction(EntityType.RangedUnit, positionToBuild), null, null));
                }
            }
        }

        private void CommandUnitsBuilders()
        {
            // var needBuildHouse = Around.PopulationFree <= 0; // todo house builds
            // var canBuildHouse = Around.Me.Resource >= Around.HouseBuildingCost;
            //
            // if (needBuildHouse && canBuildHouse)
            // {
            //     var position = new Vec2Int(builderBuilding.Position.X + 10, builderBuilding.Position.Y + 10);
            //     Actions.Add(builderBuilding.Id, new EntityAction(null, new BuildAction(EntityType.House, position), null, null));
            // }

            foreach (var builderUnit in Around.MyUnitsBuilders)
            {
                var nearestSpice = Around.GetNearestTo(builderUnit, PlayerType.My, EntityType.Resource);

                var moveAction = new MoveAction(nearestSpice.Position, true, false);
                var attackAction = new AttackAction(nearestSpice.Id, null);

                var action = new EntityAction(moveAction, null, attackAction, null);

                Actions.Add(builderUnit.Id, action);
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

                Actions.Add(rangedUnit.Id, action);
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

                Actions.Add(meleeUnit.Id, action);
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

            if (Around.EnemyUnitsBuilders.Any())
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

            if (Around.EnemyBuildingsBuilders.Any())
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