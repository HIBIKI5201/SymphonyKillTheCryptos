using Cryptos.Runtime.Entity.Ingame.Word;
using System;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.Ingame.Word
{
    /// <summary>
    ///     ワードエンティティを生成する。
    /// </summary>
    public static class WordGenerator
    {
        public static WordEntityViewModel GetWordEntity(string word = "void")
        {
            WordEntity wordEntity = new(word, 1, 1);
            WordEntityViewModel wordEntityViewModel = new(wordEntity);
            
            return wordEntityViewModel;
        }
    }

    public readonly struct WordEntityViewModel
    {
        public WordEntityViewModel(WordEntity wordEntity)
        {
            _entity = wordEntity;
        }

        public event Action OnComplete
        {
            add  => _entity.OnComplete += value;
            remove => _entity.OnComplete -= value;
        }

        public event Action<string, int> OnWordUpdated
        {
            add => _entity.OnWordUpdated += value;
            remove => _entity.OnWordUpdated -= value;
        }

        public void InputChar(char c) => _entity.OnInputChar(c);

        private readonly WordEntity _entity;
    }
}
