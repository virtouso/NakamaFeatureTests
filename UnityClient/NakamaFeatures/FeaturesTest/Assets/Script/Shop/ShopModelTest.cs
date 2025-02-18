using System;
using System.Collections.Generic;
using Script.Gameplay;

namespace Script.Shop
{
    public class Asset
    {
        public string Id { get; set; }
        //   public string DisplayName { get; set; }
        // public bool Enabled { get; set; }
    }

    public class ShopPack
    {
        public string Id { get; set; }

        //   public bool Enabled { get; set; }
        public List<string> Assets { get; set; }
        public Dictionary<string, int> Resources { get; set; } = new();
    }

    public abstract class BaseShopItem
    {
        public string Id { get; set; }
        public int MarketPriority { get; set; }

        public bool Enabled { get; set; }
        public string ShopPackId { get; set; }

        public string Title { get; set; }

        //  public int MaxPermittedOnPlayerAccount { get; set; }
        public string Description { get; set; }
        public bool IsSpecialOffer { get; set; }

        public List<string> Tags { get; set; } = new();
        //   public List<string> CategoryTags { get; set; } = new();
        // public DateTime StartTime { get; set; }
        // public DateTime EndTime { get; set; }
    }

    public class ExtraFree
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public List<string> Assets { get; set; }
        public Dictionary<string, int> ExtraFreeResources { get; set; } = new();
    }

    public class VirtualCurrencyShopItem : BaseShopItem
    {
        public Dictionary<string, long> Prices = new();
    }

    public class RealCurrencyShopItem : BaseShopItem
    {
        public long Price { get; set; }
        //   public Dictionary<string, long> Prices = new();// currency -> price.
    }


    public abstract class Discount
    {
        public string Id { get; set; }
        public string DisplayPercent { get; set; }
        public int FinalPrice { get; set; }
        public bool Enabled { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class DiscountTimeLimited : Discount
    {
        public string Id { get; set; }

        //    public bool Enabled { get; set; }
        //   public DateTime StartTime { get; set; }
        //   public DateTime EndTime { get; set; }
        public int FinalPrice { get; set; }
    }


    public class DiscountCountLimited : Discount
    {
        public string Id { get; set; }

        public string Count { get; set; }
        //    public bool Enabled { get; set; }
    }

    public class DiscountCountAndTimeLimited : Discount
    {
        public string Id { get; set; }

        public string Count { get; set; }
        //     public bool Enabled { get; set; }
        //     public DateTime StartTime { get; set; }
        //     public DateTime EndTime { get; set; }
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


    public class AbTest
    {
        public string Id { get; set; }
        public string TestId { get; set; }

        public string GroupId { get; set; }
        //   public bool Enabled { get; set; }
    }


    public class StoreItemAggregate
    {
        public string Id { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public DiscountType DiscountType { get; set; }
        public ShopPack ShopPack { get; set; }
        public VirtualCurrencyShopItem VirtualCurrencyShopItem { get; set; }
        public RealCurrencyShopItem RealCurrencyShopItem { get; set; }
        public DiscountTimeLimited DiscountTimeLimited { get; set; }
        public DiscountCountLimited DiscountCountLimited { get; set; }
        public ExtraFree ExtraFree { get; set; }
        public List<string> Conditions { get; set; } = new();
        public bool Enabled { get; set; }
        public AbTest AbTest { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }


    public static class PurchaseStateConsts
    {
        public const string TransactionStatePurchased = "purchased";
        public const string TransactionStateValidated = "validated";
        public const string TransactionStateApplied = "applied";
        public const string TransactionStateConsumed = "consumed";
    }


    public static class AppPlacesConsts
    {
        public const string StartPopUp = "start_popup";
        public const string Store = "store";
    }


    public enum PaymentMethod
    {
        RealCurrency,
        VirtualCurrency
    }

    public enum DiscountType
    {
        None,
        TimeLimited,
        CountLimited,
        Both
    }
}