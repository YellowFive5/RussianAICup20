#region Usings

using System;
using System.IO;

#endregion

namespace Aicup2020.Model
{
    public struct BuildProperties
    {
        public EntityType[] Options { get; set; }
        public int? InitHealth { get; set; }

        public BuildProperties(EntityType[] options, int? initHealth)
        {
            Options = options;
            InitHealth = initHealth;
        }

        public static BuildProperties ReadFrom(BinaryReader reader)
        {
            var result = new BuildProperties();
            result.Options = new EntityType[reader.ReadInt32()];
            for (int i = 0; i < result.Options.Length; i++)
            {
                switch (reader.ReadInt32())
                {
                    case 0:
                        result.Options[i] = EntityType.Wall;
                        break;
                    case 1:
                        result.Options[i] = EntityType.House;
                        break;
                    case 2:
                        result.Options[i] = EntityType.BuilderBase;
                        break;
                    case 3:
                        result.Options[i] = EntityType.BuilderUnit;
                        break;
                    case 4:
                        result.Options[i] = EntityType.MeleeBase;
                        break;
                    case 5:
                        result.Options[i] = EntityType.MeleeUnit;
                        break;
                    case 6:
                        result.Options[i] = EntityType.RangedBase;
                        break;
                    case 7:
                        result.Options[i] = EntityType.RangedUnit;
                        break;
                    case 8:
                        result.Options[i] = EntityType.Resource;
                        break;
                    case 9:
                        result.Options[i] = EntityType.Turret;
                        break;
                    default:
                        throw new Exception("Unexpected tag value");
                }
            }

            if (reader.ReadBoolean())
            {
                result.InitHealth = reader.ReadInt32();
            }
            else
            {
                result.InitHealth = null;
            }

            return result;
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(Options.Length);
            foreach (var OptionsElement in Options)
            {
                writer.Write((int) (OptionsElement));
            }

            if (!InitHealth.HasValue)
            {
                writer.Write(false);
            }
            else
            {
                writer.Write(true);
                writer.Write(InitHealth.Value);
            }
        }
    }
}