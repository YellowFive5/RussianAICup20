#region Usings

using System.IO;

#endregion

namespace Aicup2020.Model
{
    public struct Player
    {
        public int Id { get; set; }
        public int Score { get; set; }
        public int Resource { get; set; }

        public Player(int id, int score, int resource)
        {
            Id = id;
            Score = score;
            Resource = resource;
        }

        public static Player ReadFrom(BinaryReader reader)
        {
            var result = new Player();
            result.Id = reader.ReadInt32();
            result.Score = reader.ReadInt32();
            result.Resource = reader.ReadInt32();
            return result;
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(Id);
            writer.Write(Score);
            writer.Write(Resource);
        }
    }
}