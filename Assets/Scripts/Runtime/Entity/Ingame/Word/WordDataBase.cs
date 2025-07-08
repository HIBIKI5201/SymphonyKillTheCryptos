using Cryptos.Runtime.System;
using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Word
{
    /// <summary>
    ///     ワードのデータベース
    /// </summary>
    [CreateAssetMenu(fileName = nameof(WordDataBase),
        menuName = CryptosPathConstant.ASSET_PATH + nameof(WordDataBase))]
    public class WordDataBase : ScriptableObject
    {
        public string[] this[int index] => _words[index].Words;

        public WordData[] WordData => _words;

        [SerializeField]
        private WordData[] _words;
    }
}