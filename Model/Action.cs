#region Usings

using System.Collections.Generic;
using System.IO;

#endregion

namespace Aicup2020.Model
{
    public struct Action
    {
        public IDictionary<int, EntityAction> EntityActions { get; set; }

        public Action(IDictionary<int, EntityAction> entityActions)
        {
            EntityActions = entityActions;
        }

        public static Action ReadFrom(BinaryReader reader)
        {
            var result = new Action();
            int EntityActionsSize = reader.ReadInt32();
            result.EntityActions = new Dictionary<int, EntityAction>(EntityActionsSize);
            for (int i = 0; i < EntityActionsSize; i++)
            {
                int EntityActionsKey;
                EntityActionsKey = reader.ReadInt32();
                EntityAction EntityActionsValue;
                EntityActionsValue = EntityAction.ReadFrom(reader);
                result.EntityActions.Add(EntityActionsKey, EntityActionsValue);
            }

            return result;
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(EntityActions.Count);
            foreach (var EntityActionsEntry in EntityActions)
            {
                var EntityActionsKey = EntityActionsEntry.Key;
                var EntityActionsValue = EntityActionsEntry.Value;
                writer.Write(EntityActionsKey);
                EntityActionsValue.WriteTo(writer);
            }
        }
    }
}