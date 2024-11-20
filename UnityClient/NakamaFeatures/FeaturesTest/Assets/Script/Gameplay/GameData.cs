using System.Collections.Generic;

namespace Script.Gameplay
{
    public class GameData
    {
        public List<Round> Rounds { get; set; } = new();
    }

    public class Round
    {
        public Dictionary<string, PlayerRoundData> Players { get; set; } = new();
    }


    public class PlayerRoundData
    {
        public string UserId { get; set; }
        public int GuessNumber { get; set; }
        public int SecretNumber { get; set; }
        public bool RightGuess { get; set; }
    }
    
}