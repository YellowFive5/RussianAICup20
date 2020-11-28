#region Usings

using System;
using System.IO;

#endregion

namespace Aicup2020.Model
{
    public abstract class ServerMessage
    {
        public abstract void WriteTo(BinaryWriter writer);

        public static ServerMessage ReadFrom(BinaryReader reader)
        {
            switch (reader.ReadInt32())
            {
                case GetAction.TAG:
                    return GetAction.ReadFrom(reader);
                case Finish.TAG:
                    return Finish.ReadFrom(reader);
                case DebugUpdate.TAG:
                    return DebugUpdate.ReadFrom(reader);
                default:
                    throw new Exception("Unexpected tag value");
            }
        }

        public class GetAction : ServerMessage
        {
            public const int TAG = 0;
            public PlayerView PlayerView { get; set; }
            public bool DebugAvailable { get; set; }

            public GetAction()
            {
            }

            public GetAction(PlayerView playerView, bool debugAvailable)
            {
                PlayerView = playerView;
                DebugAvailable = debugAvailable;
            }

            public static new GetAction ReadFrom(BinaryReader reader)
            {
                var result = new GetAction();
                result.PlayerView = PlayerView.ReadFrom(reader);
                result.DebugAvailable = reader.ReadBoolean();
                return result;
            }

            public override void WriteTo(BinaryWriter writer)
            {
                writer.Write(TAG);
                PlayerView.WriteTo(writer);
                writer.Write(DebugAvailable);
            }
        }

        public class Finish : ServerMessage
        {
            public const int TAG = 1;

            public static new Finish ReadFrom(BinaryReader reader)
            {
                var result = new Finish();
                return result;
            }

            public override void WriteTo(BinaryWriter writer)
            {
                writer.Write(TAG);
            }
        }

        public class DebugUpdate : ServerMessage
        {
            public const int TAG = 2;
            public PlayerView PlayerView { get; set; }

            public DebugUpdate()
            {
            }

            public DebugUpdate(PlayerView playerView)
            {
                PlayerView = playerView;
            }

            public static new DebugUpdate ReadFrom(BinaryReader reader)
            {
                var result = new DebugUpdate();
                result.PlayerView = PlayerView.ReadFrom(reader);
                return result;
            }

            public override void WriteTo(BinaryWriter writer)
            {
                writer.Write(TAG);
                PlayerView.WriteTo(writer);
            }
        }
    }
}