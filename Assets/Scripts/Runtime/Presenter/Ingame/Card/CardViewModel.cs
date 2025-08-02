using Cryptos.Runtime.Entity.Ingame.Card;
using System;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.Ingame.Card
{
    public struct CardViewModel
    {
        public CardViewModel(CardEntity cardEntity)
        {
            _cardEntity = cardEntity;
        }

        public event Action<string, int> OnWordUpdated
        {
            add => _cardEntity.WordEntity.OnWordUpdated += value;
            remove => _cardEntity.WordEntity.OnWordUpdated -= value;
        }

        public event Action<float> OnProgressUpdate
        {
            add => _cardEntity.WordEntity.OnProgressUpdate += value;
            remove => _cardEntity.WordEntity.OnProgressUpdate -= value;
        }

        public string CurrentWord => _cardEntity.WordEntity.CurrentWord;

        private CardEntity _cardEntity;
    }
}
