using System.Collections.Generic;

namespace Script.NonAuthoritative
{
    public class OpCodeNames
    {
        public const long ExitMatch = 1;
        public const long StartMatch = 2;
        public const long UpdateTurn = 3;
        public const long CheckAnswer = 4;
    }


    public class MessageData
    {
       public string UserId { get; set; }
       public string Username { get; set; }
       public long OpCode { get; set; }
       public Dictionary<string,string> Messages { get; set; }
    }

    
    public class UserPresenceData
    {
        public bool Join { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
    }
    
}