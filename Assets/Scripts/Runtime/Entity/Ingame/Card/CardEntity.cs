using Cryptos.Runtime.Entity.Ingame.Word;
using System;
using UnityEngine;

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

        /// <summary> このカードに関連付けられたワードタイピングのエンティティを取得します。 </summary>
        public WordEntity WordEntity => _word;

        /// <summary> アニメーションクリップのIDを取得します。 </summary>
        public int AnimationClipID => _data.AnimationClipID;

        /// <summary> カードのアイコン </summary>
        public Texture2D Icon => _data.CardIcon;

        /// <summary>
        /// 指定されたインデックスのカード効果を取得します。
        /// </summary>
        /// <param name="index">取得する効果のインデックス。</param>
        /// <returns>カード効果の配列。</returns>
        public ICardContent[] GetContents(int index)
        {
            if (index < 0 || index >= _data.ContentsArray.Length)
            {
                throw new IndexOutOfRangeException($"Index {index} is out of range for contents array.\ndata {_data.name}");
            }

            return _data.ContentsArray[index].Contents;
        }

        private readonly CardData _data;
        private readonly WordEntity _word;
    }
}