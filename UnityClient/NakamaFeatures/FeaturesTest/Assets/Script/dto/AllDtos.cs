using System.Collections.Generic;
using Newtonsoft.Json;

namespace Script.dto
{
    public class SendCardConfigRequest
    {
        [JsonProperty("secret")]
        public string Secret { get; set; }
    
        [JsonProperty("jsonConfig")]
        public CardConfig JsonConfig { get; set; }
    
        [JsonProperty("cardId")]
        public string CardId { get; set; }
    }

    public class CardConfig
    {
        [JsonProperty("cardId")]
        public string CardId { get; set; }
        [JsonProperty("story")]
        public string Story { get; set; }
    
        [JsonProperty("mana")]
        public int Mana { get; set; }
    
        [JsonProperty("health")]
        public int Health { get; set; }
    
        [JsonProperty("attack")]
        public int Attack { get; set; }
    
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
    
        [JsonProperty("buyPrice")]
        public Dictionary<string, long> BuyPrice { get; set; }
    
        [JsonProperty("sellPrice")]
        public Dictionary<string, long> SellPrice { get; set; }
    
        [JsonProperty("marketable")]
        public bool Marketable { get; set; }
    
        [JsonProperty("rarity")]
        public string Rarity { get; set; }
    }
}