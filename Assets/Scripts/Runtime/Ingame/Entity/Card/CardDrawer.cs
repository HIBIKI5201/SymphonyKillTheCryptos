using UnityEngine;

namespace Cryptos.Runtime.Ingame.Entity
{
    public class CardDrawer
    {
        public CardDrawer(WordManager wordManager)
        {
            _wordManager = wordManager;
        }

        [SerializeField, Tooltip("ワードのデータベース")]
        private WordDataBase _wordDatabase;

        private readonly WordManager _wordManager;

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