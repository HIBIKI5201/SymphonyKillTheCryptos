using Cryptos.Runtime.Framework;
using System;
using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Card
{
    /// <summary>
    /// ゲーム内の「カード」の静的なデータ情報を持つScriptableObject。
    /// カードの名前、説明、難易度、アイコン、ワードの範囲、およびカード効果の配列を定義します。
    /// </summary>
    [CreateAssetMenu(fileName = nameof(CardData),
        menuName = CryptosPathConstant.ASSET_PATH + nameof(CardData))]
    public class CardData : ScriptableObject
    {
        /// <summary>
        /// カードの名前を取得します。
        /// </summary>
        public string CardName => _cardName;

        /// <summary>
        /// カードの説明を取得します。
        /// </summary>
        public string CardExplanation => _cardExplanation;

        /// <summary>
        /// カードの難易度を取得します。
        /// </summary>
        public int CardDifficulty => _cardDifficulty;

        /// <summary>
        /// カードのアイコン画像を取得します。
        /// </summary>
        public Texture2D CardIcon => _cardIcon;

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


        [Header("基本情報")]
        [SerializeField, Tooltip("カードの名前")]
        private string _cardName = "empty";

        [SerializeField, Tooltip("カードの説明"), TextArea]
        private string _cardExplanation = "explanation";

        [SerializeField, Tooltip("アイコン画像")]
        private Texture2D _cardIcon = default;

        [Header("カードの詳細設定")]
        [SerializeField, Tooltip("カードの難易度"), Min(1)]
        private int _cardDifficulty = 1;

        [SerializeField, Tooltip("ワードの範囲")]
        private Vector2Int _wordRange = new Vector2Int(0, 1);

        [SerializeField]
        private CardContents[] _contentsArray = default;
        [SerializeField, Tooltip("カード優先度")]
        private int _priority = 0;

        [Header("演出設定")]
        [SerializeField, Tooltip("アニメーションのID")]
        private int _animationClipID = default;

        [Serializable]
        public class CardContents
        {
            public ICardContent this[int index] => _content[index];

            public ICardContent[] Contents => _content;

            [SerializeReference, SubclassSelector]
            private ICardContent[] _content;
        }
    }
}
