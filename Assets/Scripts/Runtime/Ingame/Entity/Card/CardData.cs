using Cryptos.Runtime.System;
using UnityEngine;

namespace Cryptos.Runtime.Ingame.Entity
{
    /// <summary>
    ///     カードのデータ情報
    /// </summary>
    [CreateAssetMenu(fileName = nameof(CardData),
        menuName = CryptosPathConstant.ASSET_PATH + nameof(CardData))]
    public class CardData : ScriptableObject
    {
        public string CardName => _cardName;
        public string CardExplanation => _cardExplanation;
        public int CardDifficulty => _cardDifficulty;
        public Texture2D CardIcon => _cardIcon;
        public Vector2Int WordRange => _wordRange;
        public ICardContent[] Contents => _contentsArray;


        [Header("基本情報")]
        [SerializeField, Tooltip("カードの名前")]
        private string _cardName;

        [SerializeField, Tooltip("カードの説明"), TextArea]
        private string _cardExplanation;

        [SerializeField, Tooltip("アイコン画像")]
        private Texture2D _cardIcon;

        [SerializeField, Tooltip("カードの難易度"), Min(1)]
        private int _cardDifficulty;

        [SerializeField, Tooltip("ワードの範囲")]
        private Vector2Int _wordRange = new Vector2Int(0, 1);

        [SerializeReference, SubclassSelector]
        private ICardContent[] _contentsArray;
    }
}