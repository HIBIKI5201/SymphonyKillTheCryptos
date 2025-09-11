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
        private const int DEFAULT_DIFFICULTY = 1;

        /// <summary>
        ///     ワードエンティティのビューモデルを生成する。
        /// </summary>
        /// <param name="word">生成するワード。</param>
        /// <returns>生成されたワードエンティティのビューモデル。</returns>
        public static WordEntityViewModel GetWordEntity(string word = "void")
        {
            WordEntity wordEntity = new(word, DEFAULT_DIFFICULTY, DEFAULT_DIFFICULTY);
            WordEntityViewModel wordEntityViewModel = new(wordEntity);
            
            return wordEntityViewModel;
        }
    }
}
