using System;
using System.Collections.Generic;
using System.Linq;

namespace Script
{
  namespace dsadsadsa
{
    class HeroSlot
    {
        public string Hero { get; set; }
        public int Slot { get; set; }
    }

    class CardReference
    {
        public string CardId { get; set; }
        public int BuyPrice { get; set; }
        public int SellPrice { get; set; }
        public bool Marketable { get; set; }
        public int MaxInDeck { get; set; }
    }


    class Card
    {
        public string CardId { get; set; }

        public List<HeroSlot> Decks { get; set; } = new List<HeroSlot>();
    }


    class CardManager
    {
        public List<CardReference> CardRefs = new List<CardReference>();
        public List<Card> Cards { get; set; } = new List<Card>();

        public void Buy(string cardId)
        {
            Cards.Add(new Card { CardId = cardId, });
        }

        public void Sell(string cardId)
        {
            var refCard = Cards.LastOrDefault(c => c.CardId == cardId);
            Cards.Remove(refCard);
        }

        public void AddToDeck(string cardId, string heroId, int slot)
        {
            var cardRef = CardRefs.FirstOrDefault(c => c.CardId == cardId);
            var cards = Cards.FindAll(card => card.CardId == cardId);
            var cardTypeInCurrentDeck = Cards.Where(x => x.CardId == cardId)
                .Where(x => x.Decks.FirstOrDefault(y => y.Hero == heroId && y.Slot == slot) != null).ToList();

            if (cardTypeInCurrentDeck.Count >= cardRef.MaxInDeck)
            {
                throw new Exception("Deck is full for this cards");
                return;
            }

            cards.First(x =>
                    x.CardId == cardId && x.Decks.FirstOrDefault(y => y.Hero == heroId && y.Slot == slot) == null).Decks
                .Add(new HeroSlot() { Hero = heroId, Slot = slot });
        }

        public List<Card> GetDeckCards(string heroId, int slot)
        {
            return Cards.Where(x => x.Decks.FirstOrDefault(y => y.Hero == heroId && y.Slot == slot) != null).ToList();
        }

        public void CreateCardRef(string cardId, int buyPrice, int sellPrice, bool marketable, int maxInDeck)
        {
            CardRefs.Add(new CardReference
            {
                CardId = cardId, BuyPrice = buyPrice, SellPrice = sellPrice, Marketable = marketable,
                MaxInDeck = maxInDeck
            });
        }
    }


    internal class Program
    {
        static CardManager _cardManager = new CardManager();
        static Random _rnd = new Random();

        public static void Main(string[] args)
        {
            InitReferences();
            BuyRandomCards();
        }

        private static void BuyRandomCards()
        {
            // _cardManager.Buy(_cardManager.CardRefs.ElementAt(_rnd.Next(0, _cardManager.CardRefs.Count)).CardId);
            //
            // _cardManager.Buy(_cardManager.CardRefs.ElementAt(_rnd.Next(0, _cardManager.CardRefs.Count)).CardId);
            // _cardManager.Buy(_cardManager.CardRefs.ElementAt(_rnd.Next(0, _cardManager.CardRefs.Count)).CardId);
            // _cardManager.Buy(_cardManager.CardRefs.ElementAt(_rnd.Next(0, _cardManager.CardRefs.Count)).CardId);

            _cardManager.Buy("changiz");
            _cardManager.Buy("changiz");

            _cardManager.AddToDeck("changiz", "booz", 1);
            _cardManager.AddToDeck("changiz", "booz", 1);
            var res = _cardManager.GetDeckCards("booz",1);
            _cardManager.Sell("changiz");
            
            res = _cardManager.GetDeckCards("changiz", 1);
            
        }

        private static void InitReferences()
        {
            _cardManager.CreateCardRef("changiz", 10, 10, true, 2);
            _cardManager.CreateCardRef("teymoor", 10, 10, true, 2);
            _cardManager.CreateCardRef("golmorad", 10, 10, true, 2);
            _cardManager.CreateCardRef("boontoon", 10, 10, true, 2);
        }
    }
}
}