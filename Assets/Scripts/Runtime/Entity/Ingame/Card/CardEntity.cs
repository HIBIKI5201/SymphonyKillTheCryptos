using Cryptos.Runtime.Entity.Ingame.Word;
using System;

namespace Cryptos.Runtime.Entity.Ingame.Card
{
    /// <summary>
    /// ゲーム内の「カード」のインスタンスデータを表すエンティティクラス。
    /// カードの静的なデータと、それに紐づくワードタイピングのインスタンスを保持します。
    /// </summary>
    public class CardEntity
    {
        /// <summary>
        /// CardEntityの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="data">このカードの静的なデータ（CardData ScriptableObject）</param>
        /// <param name="word">このカードに関連付けられたワードタイピングのエンティティ</param>
        public CardEntity(CardData data, WordEntity word)
        {
            _data = data;
            _word = word;
        }

        /// <summary>
        /// ワードタイピングが完了したときに発生するイベント。
        /// WordEntityのOnCompleteイベントをプロキシします。
        /// </summary>
        public event Action OnComplete
        {
            add => _word.OnComplete += value;
            remove => _word.OnComplete -= value;
        }

        /// <summary>
        /// このカードの静的なデータ（CardData ScriptableObject）を取得します。
        /// </summary>
        public CardData Data => _data;

        /// <summary>
        /// このカードに関連付けられたワードタイピングのエンティティを取得します。
        /// </summary>
        public WordEntity WordEntity => _word;

        private readonly CardData _data;
        private readonly WordEntity _word;
    }
}