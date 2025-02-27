using System;
using System.Collections.Generic;
using UnityEngine;

namespace Script.config
{
    public class CardConfig : ScriptableObject
    {
        [field: SerializeField] public int Id { get; private set; }
        [field: SerializeField] public string DisplayName { get; private set; }

        [field: SerializeField] public int Health { get; private set; }
        [field: SerializeField] public int Mana { get; private set; }
        [field: SerializeField] public bool Marketable { get; private set; }
        [field: SerializeField] public string Rarity { get; private set; }
        [field: SerializeField] public int OpenPackChance { get; private set; }
        [field: SerializeField] public int Attack { get; private set; }
        [field: SerializeField] public string Story { get; private set; }
        [field: SerializeField] public List<CurrencyData> BuyPrice { get; private set; }
        [field: SerializeField] public List<CurrencyData> SellPrice { get; private set; }


        [Serializable]
        public struct CurrencyData
        {
            [field: SerializeField] public string CurrencyId { get; private set; }
            [field: SerializeField] public long Amount { get; private set; }
        }
    }
}