using Cryptos.Runtime.Entity.Ingame.Word;
using System;
using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Card
{
    /// <summary>
    ///     カードを生成するクラス
    /// </summary>
    [Serializable]
    public class CardDrawer
    {
        [SerializeField, Tooltip("ワードのデータベース")]
        private WordDataBase _wordDatabase;

        private readonly WordManager _wordManager = new();

        /// <summary>
        ///     新しいカードを生成する
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public CardInstance GetNewCard(CardData data)
        {
            if (Mathf.Abs(data.WordRange.x - data.WordRange.y) < 1)
            {
                Debug.LogWarning($"{data.name}のWordRnageが不適切です\n二つの距離は0から{_wordDatabase.WordData.Length - 1}までです");
                return null;
            }

            //カードの難易度までのワードを取得
            WordData[] words = _wordDatabase
                .WordData[data.WordRange.x..data.WordRange.y];

            //カードを生成
            CardInstance instance = new(data, words, _wordManager);

            return instance;
        }
    }
}