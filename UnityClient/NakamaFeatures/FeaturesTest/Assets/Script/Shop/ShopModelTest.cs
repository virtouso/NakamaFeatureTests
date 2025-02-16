using System;
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

    public class ShopPack
    {
        public string Id { get; set; }
        public List<string> Assets { get; set; }
        public Dictionary<string,int> Resources { get; set; }
        
    }
    
    public abstract class BaseShopItem
    {
        public string Id { get; set; }
        public int MarketPriority { get; set; }
        
        public bool Enabled { get; set; }
        public string ShopPackId { get; set; }
        public string Title { get; set; }
        public int MaxPermittedOnPlayerAccount { get; set; }
        public Dictionary<string, int> ExtraFreeResources { get; set; }
        public string Description { get; set; }
    }



    public class ExtraFree
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public List<string> Assets { get; set; }
        public Dictionary<string, int> ExtraFreeResources { get; set; }
    }

    public class VirtualCurrencyShopItem
    {
        public Dictionary<string, long> Prices = new ();
    }

    public class RealCurrencyShopItem
    {
        public long Price { get; set; }
    }


    public abstract class Discount
    {
        public string Id { get; set; }
        public string DisplayPercent { get; set; }
        public int FinalPrice { get; set; }
        public bool Enabled { get; set; }
        public string Title { get; set; }
    }

    public class DiscountTimeLimited : Discount
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }


    public class DiscountCountLimited : Discount
    {
        public string Id { get; set; }
        public string Count { get; set; }
        public bool Enabled { get; set; }
    }

    public class DiscountCountLimitedState
    {
        public string Id { get; set; }
        public string DiscountId { get; set; }
        public int Remaining { get; set; }
    }

    public abstract class BaseCondition
    {
        public string Id { get; set; }

        // more properties
        public abstract bool PlayerCanSee(PlayerProgress progress);
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


    static class Consts
    {
        public static string TransactionStatePurchased = "purchased";
        public static string TransactionStateApplied = "applied";
        public static string TransactionStateConsumed = "consumed";
    }
}