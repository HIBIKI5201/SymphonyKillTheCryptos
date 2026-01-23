using Cryptos.Runtime.Entity.Ingame.Card;
using Cryptos.Runtime.Framework;
using Cryptos.Runtime.UseCase.Ingame.Card;
using UnityEngine;
using static Cryptos.Runtime.Entity.Ingame.Card.CardData;

namespace Cryptos.Runtime.InfraStructure.Ingame.DataAsset
{
    /// <summary>
    ///     カードデータのデータアセット
    /// </summary>
    [CreateAssetMenu(fileName = nameof(CardData),
    menuName = CryptosPathConstant.ASSET_PATH + nameof(CardData))]
    public class CardDataAsset : ScriptableObject
    {
        public CardData CreateCardData(CombatPipelineAsset combatPipelineAsset)
        {
            CardData data = new(
                _cardName,
                _cardExplanation,
                _cardIcon,
                _cardDifficulty,
                _wordRange,
                _contentsArray,
                _animationClipID);

            foreach (var cardContents in data.ContentsArray)
            {
                foreach (var content in cardContents.Contents)
                {
                    if (content is CardContentBaseAttack attack)
                    {
                        attack.InitializeCombatHandler(combatPipelineAsset.CombatHandler);
                    }
                }
            }

            return data;
        }

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

        [SerializeField, Tooltip("このカードが持つ効果の配列。")]
        private CardContents[] _contentsArray = default;

        [Header("演出設定")]
        [SerializeField, Tooltip("アニメーションのID")]
        private int _animationClipID = default;
    }
}
