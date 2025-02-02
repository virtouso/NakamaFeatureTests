using System.Collections.Generic;
using UnityEngine;

namespace Script.config
{
    public class CardsList : ScriptableObject
    {
        [field: SerializeField] public List<CardConfig> Cards { get; private set; }
    }
}