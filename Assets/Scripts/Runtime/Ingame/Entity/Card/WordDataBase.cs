using Cryptos.Runtime.System;
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
        public WordData[][] WordData => _words;

        [SerializeField]
        private WordData[][] _words;
    }
}