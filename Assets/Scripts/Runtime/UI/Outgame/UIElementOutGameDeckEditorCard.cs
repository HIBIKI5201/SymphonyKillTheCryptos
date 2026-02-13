using Cryptos.Runtime.Presenter.OutGame;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;


namespace Cryptos.Runtime.UI.Outgame.Deck
{
    [UxmlElement]
    public partial class UIElementOutGameDeckEditorCard : VisualElementBase
    {
        public UIElementOutGameDeckEditorCard() : base("DeckEditorCard", InitializeType.Absolute) { }

        public CardViewModel CardData { get; private set; }

        public void BindCardData(CardViewModel card)
        {
            CardData = card; // プロパティに設定
            _icon.style.backgroundImage = card.CardIcon;
            _explanation.text = card.CardExplanation;
        }

        protected override ValueTask Initialize_S(VisualElement root)
        {
            _icon = root.Q<VisualElement>(ICON_NAME);
            _explanation = root.Q<Label>(EXPLANATION_NAME);
            return default;
        }

        private const string EXPLANATION_NAME = "explanation";
        private const string ICON_NAME = "icon";

        private VisualElement _icon;
        private Label _explanation;
    }
}
