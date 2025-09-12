using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.Framework;
using System;
using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Card
{
    /// <summary>
    /// ゲーム内のカードの静的なデータ情報を持つクラス。
    /// カードの名前、説明、難易度、アイコン、ワードの範囲、およびカード効果の配列を定義します。
    /// </summary>
    public class CardData : ScriptableObject
    {
        public CardData(
            string cardName,
            string cardExplanation,
            Texture2D cardIcon,
            int cardDifficulty,
            Vector2Int wordRange,
            CardContents[] contentsArray,
            int priority,
            int animationClipID)
        {
            _cardName = cardName;
            _cardExplanation = cardExplanation;
            _cardIcon = cardIcon;
            _cardDifficulty = cardDifficulty;
            _wordRange = wordRange;
            _contentsArray = contentsArray;
            _priority = priority;
            _animationClipID = animationClipID;
        }

        /// <summary>
        /// カードの名前を取得します。
        /// </summary>
        public string CardName => _cardName;

        /// <summary>
        /// カードの説明を取得します。
        /// </summary>
        public string CardExplanation => _cardExplanation;

        /// <summary>
        /// カードのアイコン画像を取得します。
        /// </summary>
        public Texture2D CardIcon => _cardIcon;

        /// <summary>
        /// カードの難易度を取得します。
        /// </summary>
        public int CardDifficulty => _cardDifficulty;

        /// <summary>
        /// ワードデータベースからワードを選択する範囲（x:開始インデックス, y:終了インデックス）を取得します。
        /// </summary>
        public Vector2Int WordRange => _wordRange;

        /// <summary>
        /// このカードが持つ効果の配列を取得します。
        /// </summary>
        public CardContents[] ContentsArray => _contentsArray;

        /// <summary>
        /// カードの優先度を取得します。
        /// </summary>
        public int Priority => _priority;

        /// <summary>
        /// カードに関連付けられたアニメーションのIDを取得します。
        /// </summary>
        public int AnimationClipID => _animationClipID;

        private string _cardName = "empty";
        private string _cardExplanation = "explanation";
        private Texture2D _cardIcon = default;
        private int _cardDifficulty = 1;
        private Vector2Int _wordRange = new Vector2Int(0, 1);
        private CardContents[] _contentsArray = default;
        private int _priority = 0;
        private int _animationClipID = default;

        /// <summary>
        /// カード効果の配列を保持するクラス。
        /// </summary>
        [Serializable]
        public class CardContents
        {
            /// <summary>
            /// 指定されたインデックスのカード効果を取得します。
            /// </summary>
            public ICardContent this[int index] => _content[index];

            /// <summary>
            /// このカードが持つ効果の配列を取得します。
            /// </summary>
            public ICardContent[] Contents => _content;

            public void ExcuteAllContent(ICharacter[] players, ICharacter[] enemies)
            {
                if (_content == null) return;

                foreach (var item in _content)
                {
                    item.Execute(players, enemies);
                }
            }

            [SerializeReference, SubclassSelector, Tooltip("カード効果の配列。")]
            private ICardContent[] _content;
        }
    }
}
