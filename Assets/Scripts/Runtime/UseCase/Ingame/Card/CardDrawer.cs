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
        /// <param name="wordManager">ワードを管理するマネージャー</param>
        public CardDrawer(WordDataBase wordDataBase, WordManager wordManager)
        {
            _wordDatabase = wordDataBase ?? throw new ArgumentNullException(nameof(wordDataBase));
            _wordManager = wordManager ?? throw new ArgumentNullException(nameof(wordManager));
        }

        /// <summary>
        /// 新しいCardEntityインスタンスを生成します。
        /// </summary>
        /// <param name="data">生成するカードのデータ</param>
        /// <param name="availableWords">[out] このカードで利用可能なワードの候補リスト</param>
        /// <returns>生成されたCardEntityインスタンス。利用可能なワードがない場合はnull。</returns>
        public CardEntity CreateNewCard(CardData data, out WordData[] availableWords)
        {
            if (Mathf.Abs(data.WordRange.x - data.WordRange.y) < 1)
            {
                Debug.LogWarning($"{data.CardName}のWordRnageが不適切です");
                availableWords = null;
                return null;
            }

            // カードで利用可能なワードの候補リストを作成
            availableWords = _wordDatabase.WordData[data.WordRange.x..data.WordRange.y];

            // 最初の単語を取得
            var initialWordData = _wordManager.GetAvailableWord(availableWords);
            if (initialWordData.word == null)
            {
                Debug.LogError("利用可能な単語が見つかりませんでした。");
                return null;
            }

            // WordEntityを最初の単語で初期化
            WordEntity wordEntity = new(initialWordData.word, initialWordData.difficulty, data.CardDifficulty);
            _wordManager.AddWord(initialWordData.word); // 使用中にする

            CardEntity instance = new(data, wordEntity);
            return instance;
        }

        private readonly WordDataBase _wordDatabase;
        private readonly WordManager _wordManager;
    }
}
