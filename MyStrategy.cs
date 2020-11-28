#region Usings

using System.Collections.Generic;
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
            CommandUnitsBuilders();

            return new Action(Actions);
        }

        private void CommandBuildingsBuilders()
        {
            foreach (var builderBuilding in Around.MyBuildingsBuilders)
            {
                var buildAction = new BuildAction();

                var canBuildBuilders = Around.Me.Resource >= Around.BuilderUnitCost;
                if (canBuildBuilders)
                {
                    var position = new Vec2Int(builderBuilding.Position.X + 5, builderBuilding.Position.Y);
                    buildAction = new BuildAction(EntityType.BuilderUnit, position);
                }

                var action = new EntityAction(null, buildAction, null, null);

                Actions.Add(builderBuilding.Id, action);
            }
        }

        private void CommandUnitsBuilders()
        {
            foreach (var builderUnit in Around.MyUnitsBuilders)
            {
                var nearestSpice = Around.GetNearestTo(builderUnit, PlayerType.My, EntityType.Resource);

                var moveAction = new MoveAction(nearestSpice.Position, true, false);
                var attackAction = new AttackAction(nearestSpice.Id, null);

                var action = new EntityAction(moveAction, null, attackAction, null);

                Actions.Add(builderUnit.Id, action);
            }
        }

        public void DebugUpdate(PlayerView playerView, DebugInterface debugInterface)
        {
            debugInterface.Send(new DebugCommand.Clear());
            debugInterface.GetState();
        }
    }
}