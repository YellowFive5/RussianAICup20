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

        public MyStrategy()
        {
            Around = new World();
        }

        public Action GetAction(PlayerView playerView, DebugInterface debugInterface)
        {
            var entitiesActions = new Dictionary<int, EntityAction>();

            View = playerView;
            Debug = debugInterface;

            Around.Scan(View);

            foreach (var builder in Around.MyUnitsBuilders)
            {
                var nearestSpice = Around.GetNearestTo(builder, PlayerType.My, EntityType.Resource);

                var moveAction = new MoveAction(nearestSpice.Position, true, false);
                var attackAction = new AttackAction(nearestSpice.Id, null);

                var action = new EntityAction(moveAction, null, attackAction, null);

                entitiesActions.Add(builder.Id, action);
            }


            return new Action(entitiesActions);
        }

        public void DebugUpdate(PlayerView playerView, DebugInterface debugInterface)
        {
            debugInterface.Send(new DebugCommand.Clear());
            debugInterface.GetState();
        }
    }
}