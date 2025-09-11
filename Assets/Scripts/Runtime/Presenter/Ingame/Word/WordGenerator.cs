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
}
