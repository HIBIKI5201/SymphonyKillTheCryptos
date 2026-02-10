using Cryptos.Runtime.Entity.Ingame.Card;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cryptos.Runtime.Entity
{
    [Serializable]
    public class PlayerDeckSaveData
    {
        [SerializeField]
        private Dictionary<string, CardDeckEntity> _attackDeck;
    }
}
