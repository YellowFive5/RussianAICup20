#region Usings

using System.IO;
using System.Text;

#endregion

namespace Aicup2020.Model
{
    public struct DebugState
    {
        public Vec2Int WindowSize { get; set; }
        public Vec2Float MousePosWindow { get; set; }
        public Vec2Float MousePosWorld { get; set; }
        public string[] PressedKeys { get; set; }
        public Camera Camera { get; set; }
        public int PlayerIndex { get; set; }

        public DebugState(Vec2Int windowSize, Vec2Float mousePosWindow, Vec2Float mousePosWorld, string[] pressedKeys, Camera camera, int playerIndex)
        {
            WindowSize = windowSize;
            MousePosWindow = mousePosWindow;
            MousePosWorld = mousePosWorld;
            PressedKeys = pressedKeys;
            Camera = camera;
            PlayerIndex = playerIndex;
        }

        public static DebugState ReadFrom(BinaryReader reader)
        {
            var result = new DebugState();
            result.WindowSize = Vec2Int.ReadFrom(reader);
            result.MousePosWindow = Vec2Float.ReadFrom(reader);
            result.MousePosWorld = Vec2Float.ReadFrom(reader);
            result.PressedKeys = new string[reader.ReadInt32()];
            for (int i = 0; i < result.PressedKeys.Length; i++)
            {
                result.PressedKeys[i] = Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt32()));
            }

            result.Camera = Camera.ReadFrom(reader);
            result.PlayerIndex = reader.ReadInt32();
            return result;
        }

        public void WriteTo(BinaryWriter writer)
        {
            WindowSize.WriteTo(writer);
            MousePosWindow.WriteTo(writer);
            MousePosWorld.WriteTo(writer);
            writer.Write(PressedKeys.Length);
            foreach (var PressedKeysElement in PressedKeys)
            {
                var PressedKeysElementData = Encoding.UTF8.GetBytes(PressedKeysElement);
                writer.Write(PressedKeysElementData.Length);
                writer.Write(PressedKeysElementData);
            }

            Camera.WriteTo(writer);
            writer.Write(PlayerIndex);
        }
    }
}