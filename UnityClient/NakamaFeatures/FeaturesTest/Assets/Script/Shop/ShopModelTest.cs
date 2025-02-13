using System.Collections.Generic;
using Script.Gameplay;

namespace Script.Shop
{
    public class Asset
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public bool Enabled { get; set; }
    }

    public abstract class BaseShopItem
    {
        public string Id { get; set; } 
        public int MarketPriority { get; set; }
        public bool Enabled { get; set; }
        public string AssetId { get; set; }
    }


    public class VirtualCurrencyShopItem
    {
      public  Dictionary<string, long> Prices = new Dictionary<string, long>();
    }

    public class RealCurrencyShopItem
    {
     public long Price { get; set; }   
    }
    

    public class Discount
    {
        public string Id { get; set; }
        public string DisplayPercent { get; set; }
        public int FinalPrice { get; set; }
        public bool Enabled { get; set; }
    }

    public class Party
    {
        public string Id { get; set; }
        public string Count { get; set; }
        public bool Enabled { get; set; }
    }

    public class PartyState
    {
        public string Id { get; set; }
        public string PartyId { get; set; }
        public int Remaining { get; set; }
    }
    
    public class Condition
    {
        public string Id { get; set; }
        // more properties
        bool PlayerCanSee(PlayerProgress progress)
        {
            return true;
        }
    }

    class AbTest
    {
        public string Id { get; set; }
        public string TestId { get; set; }
        public bool Enabled { get; set; }
        
    }

    public class Extra
    {
        public string Id { get; set; }
        public bool Enabled { get; set; }
    }
    
    public class StoreItemAggregate
    {
        public string Id { get; set; }
    }
    
    
    
private string TransactionStatePurchased = "purchased";
    
}