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
        public string[] this[int index] => _words[index].Words;

        public List<WordData> WordData => _words;

        [SerializeField]
        private List<WordData> _words;
    }
}