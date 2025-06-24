using Cryptos.Runtime.System;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cryptos.Runtime.Ingame.Entity
{
    /// <summary>
    ///     ワードのデータベース
    /// </summary>
    [CreateAssetMenu(fileName = nameof(WordDataBase),
        menuName = CryptosPathConstant.ASSET_PATH + nameof(WordDataBase))]
    public class WordDataBase : ScriptableObject
    {
        public WordData[] this[int index] => _words[index].Array;

        public List<WordDataArray> WordData => _words;

        [SerializeField]
        private List<WordDataArray> _words;

        /// <summary>
        ///     ワードデータの配列
        /// </summary>
        [Serializable]
        public class WordDataArray
        {
            public WordData[] Array => _wordDataArray;

            [SerializeField]
            private WordData[] _wordDataArray;
        }
    }
}