using System.Collections.Generic;
using Newtonsoft.Json;

namespace Script.dto
{
    public class LeaderboardResponse
    {
        [JsonProperty("records")] public List<LeaderboardPlayerModel> Records { get; set; }
        [JsonProperty("prev_cursor")] public string PrevCursor { get; set; }
        [JsonProperty("next_cursor")] public string NextCursor { get; set; }
        [JsonProperty("owner_records")] public List<LeaderboardPlayerModel> OwnerRecords { get; set; }
    }



    public class LeaderboardRequest
    {
        
       [JsonProperty("leaderboard_id")] public string LeaderboardId { get; set; }
       [JsonProperty("past_count")] public int PastCount { get; set; }
       [JsonProperty("next_cursor")] public string NextCursor { get; set; }
       [JsonProperty("prev_cursor")] public string PrevCursor { get; set; }
       [JsonProperty("owner_records")] public List<string> OwnerRecords { get; set; }
    }
        
    
    
    public class CreateTime
    {
        public int seconds { get; set; }
    }

    public class ExpiryTime
    {
        public int seconds { get; set; }
    }

    public class LeaderboardPlayerModel
    {
        public string leaderboard_id { get; set; }
        public string owner_id { get; set; }
        public Username username { get; set; }
        public int score { get; set; }
        public int num_score { get; set; }
        public string metadata { get; set; }
        public CreateTime create_time { get; set; }
        public UpdateTime update_time { get; set; }
        public ExpiryTime expiry_time { get; set; }
        public int rank { get; set; }
        public int max_num_score { get; set; }
    }

    public class UpdateTime
    {
        public int seconds { get; set; }
    }

    public class Username
    {
        public string value { get; set; }
    }
}