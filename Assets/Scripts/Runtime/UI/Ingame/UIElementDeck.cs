using Cryptos.Runtime.Entity.Ingame.Card;
using Cryptos.Runtime.Entity.Ingame.Word;
using SymphonyFrameWork.Utility;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Cryptos.Runtime.UI.Ingame
{
    /// <summary>
    ///     デッキのUI
    /// </summary>
    [UxmlElement]
    public partial class UIElementDeck : SymphonyVisualElement
    {
        public UIElementDeck() : base("UIToolKit/UXML/Ingame/Deck") { }

        private const string DECK_NAME = "deck";

        private VisualElement _deck;
        private readonly Dictionary<CardEntity, UIElementCard> _cards = new();

        protected override Task Initialize_S(TemplateContainer container)
        {
            _deck = container.Q<VisualElement>(DECK_NAME);

            return Task.CompletedTask;
        }

        /// <summary>
        ///     カードをデッキに追加する
        /// </summary>
        /// <param name="instance"></param>
        public void HandleAddCard(CardEntity instance)
        {
            //カードを追加
            UIElementCard card = new ();
            card.SetData(instance.WordEntity);

            _deck.Add(card);
            _cards.Add(instance, card);
        }

        /// <summary>
        ///     カードをデッキから削除する
        /// </summary>
        /// <param name="instance"></param>
        public void HandleRemoveCard(CardEntity instance)
        {
            if (_cards.TryGetValue(instance, out UIElementCard card))
            {
                _deck.Remove(card);
                _cards.Remove(instance);
            }
        }
    }
}