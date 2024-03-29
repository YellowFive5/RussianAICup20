#region Usings

using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Aicup2020.Model;

#endregion

namespace Aicup2020
{
    public class Runner
    {
        private readonly BinaryReader reader;
        private readonly BinaryWriter writer;

        public Runner(string host, int port, string token)
        {
            var client = new TcpClient(host, port) {NoDelay = true};
            var stream = new BufferedStream(client.GetStream());
            reader = new BinaryReader(stream);
            writer = new BinaryWriter(stream);
            var tokenData = Encoding.UTF8.GetBytes(token);
            writer.Write(tokenData.Length);
            writer.Write(tokenData);
            writer.Flush();
        }

        public void Run()
        {
            var myStrategy = new MyStrategy();
            var debugInterface = new DebugInterface(reader, writer);
            var running = true;
            while (running)
            {
                switch (ServerMessage.ReadFrom(reader))
                {
                    case ServerMessage.GetAction message:
                        new ClientMessage.ActionMessage(myStrategy.GetAction(message.PlayerView,
                                                                             message.DebugAvailable
                                                                                 ? debugInterface
                                                                                 : null)).WriteTo(writer);
                        writer.Flush();
                        break;
                    case ServerMessage.Finish message:
                        running = false;
                        break;
                    case ServerMessage.DebugUpdate message:
                        myStrategy.DebugUpdate(message.PlayerView, debugInterface);
                        new ClientMessage.DebugUpdateDone().WriteTo(writer);
                        writer.Flush();
                        break;
                    default:
                        throw new Exception("Unexpected server message");
                }
            }
        }

        public static void Main(string[] args)
        {
            string host = args.Length < 1
                              ? "127.0.0.1"
                              : args[0];
            int port = args.Length < 2
                           ? 31001
                           : int.Parse(args[1]);
            string token = args.Length < 3
                               ? "0000000000000000"
                               : args[2];
            new Runner(host, port, token).Run();
        }
    }
}