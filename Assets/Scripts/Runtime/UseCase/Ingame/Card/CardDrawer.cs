using Cryptos.Runtime.Entity.Ingame.Card;
using Cryptos.Runtime.Entity.Ingame.Word;
using System;
using UnityEngine;

namespace Cryptos.Runtime.UseCase.Ingame.Card
{
    /// <summary>
    /// カードのインスタンスを生成するファクトリークラス。
    /// CardDataとWordDataBaseに基づいてCardEntityとWordEntityを構築します。
    /// </summary>
    public class CardDrawer
    {
        /// <summary>
        /// CardDrawerの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="wordDataBase">ワードデータが格納されたデータベース</param>
        public CardDrawer(WordDataBase wordDataBase)
        {
            _wordDatabase = wordDataBase ?? throw new ArgumentNullException(nameof(wordDataBase));
        }

        private readonly WordDataBase _wordDatabase;
        private readonly WordManager _wordManager = new();

        /// <summary>
        /// 新しいCardEntityインスタンスを生成します。
        /// CardDataのWordRangeに基づいてWordEntityを生成し、CardEntityに紐付けます。
        /// </summary>
        /// <param name="data">生成するカードのデータ</param>
        /// <returns>生成されたCardEntityインスタンス。WordRangeが不適切な場合はnull。</returns>
        public CardEntity CreateNewCard(CardData data)
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
            WordEntity wordEntity = new(words, _wordManager, data.CardDifficulty);
            CardEntity instance = new(data, wordEntity);

            return instance;
        }
    }
}
