using Cryptos.Runtime.Entity.Ingame.Word;
using System;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.Ingame.Word
{
    public readonly struct WordEntityViewModel
    {
        public WordEntityViewModel(WordEntity wordEntity)
        {
            _entity = wordEntity;
        }

        public event Action OnComplete
        {
            add => _entity.OnComplete += value;
            remove => _entity.OnComplete -= value;
        }

        public event Action<string, int> OnWordUpdated
        {
            add => _entity.OnWordUpdated += value;
            remove => _entity.OnWordUpdated -= value;
        }

        public string Word => _entity.CurrentWord;

        public void InputChar(char c) => _entity.OnInputChar(c);

        private readonly WordEntity _entity;
    }
}
