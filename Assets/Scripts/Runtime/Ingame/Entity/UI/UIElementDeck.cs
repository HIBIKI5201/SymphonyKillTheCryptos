using SymphonyFrameWork.Utility;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Cryptos.Runtime.Ingame.UI
{
    [UxmlElement]
    public partial class UIElementDeck : SymphonyVisualElement
    {
        public UIElementDeck() : base("UIToolKit/UXML/Ingame/Deck") { }

        private const string DECK_NAME = "deck";

        private VisualElement _deck;

        protected override Task Initialize_S(TemplateContainer container)
        {
            _deck = container.Q<VisualElement>(DECK_NAME);

            return Task.CompletedTask;
        }

        public void AddCard()
        {
            UIElementCard card = new UIElementCard();

            _deck.Add(card);
        }
    }
}