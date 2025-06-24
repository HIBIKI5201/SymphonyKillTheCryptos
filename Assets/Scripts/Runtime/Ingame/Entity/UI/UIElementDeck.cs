using Cryptos.Runtime.Ingame.Entity;
using SymphonyFrameWork.Utility;
using System.Collections.Generic;
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
        private List<UIElementCard> _cards;

        protected override Task Initialize_S(TemplateContainer container)
        {
            _deck = container.Q<VisualElement>(DECK_NAME);

            return Task.CompletedTask;
        }

        /// <summary>
        ///     カードをデッキに追加する
        /// </summary>
        /// <param name="data"></param>
        public void AddCard(CardData data)
        {
            //カードを追加
            UIElementCard card = new UIElementCard();
            card.SetData(data);

            _deck.Add(card);
            _cards.Add(card);

            //破棄する処理を登録
            card.OnDispose += () =>
            {
                _deck.Remove(card);
                _cards.Remove(card);
            };
        }
    }
}