using Cryptos.Runtime.Entity.Ingame.Word;
using System;
using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Card
{
    /// <summary>
    ///     カードのインスタンスデータ
    /// </summary>
    public class CardEntity
    {
        public CardEntity(CardData data, WordEntity word)
        {
            _data = data;
            _word = word;
        }

        public event Action OnComplete
        {
            add => _word.OnComplete += value;
            remove => _word.OnComplete -= value;
        }

        public CardData Data => _data;
        public WordEntity WordEntity => _word;

        private readonly CardData _data;
        private readonly WordEntity _word;
    }
}
