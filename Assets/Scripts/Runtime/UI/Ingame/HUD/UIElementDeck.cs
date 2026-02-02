using Cryptos.Runtime.Presenter.Ingame.Card;
using SymphonyFrameWork.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cryptos.Runtime.UI.Ingame.Card
{
    /// <summary>
    ///     デッキのUI
    /// </summary>
    [UxmlElement]
    public partial class UIElementDeck : SymphonyVisualElement
    {
        public UIElementDeck() : base("UIToolKit/UXML/InGame/Deck") { }

        protected override Task Initialize_S(TemplateContainer container)
        {
            _deck = container.Q<VisualElement>(DECK_NAME);
            _stack = container.Q<VisualElement>(STACK_NAME);

            VisualElement overlay = new();
            overlay.style.position = Position.Absolute;
            overlay.style.flexGrow = 1;
            container.Add(overlay);
            _overlay = overlay;

            return Task.CompletedTask;
        }

        /// <summary>
        ///     カードをデッキに追加する
        /// </summary>
        /// <param name="instance"></param>
        public void HandleAddCard(CardViewModel instance)
        {
            //カードを追加
            UIElementCard card = new();
            card.SetData(instance);

            _deck.Add(card);
            _cards.Add(instance, card);
        }

        /// <summary>
        ///     カードをデッキから削除する
        /// </summary>
        /// <param name="instance"></param>
        public void HandleRemoveCard(CardViewModel instance)
        {
            if (_cards.TryGetValue(instance, out UIElementCard card))
            {
                _deck.Remove(card);
                _cards.Remove(instance);
            }
        }

        public void MoveCardToStack(CardViewModel instance)
        {
            if (!_cards.TryGetValue(instance, out UIElementCard card)) { return; }

            // レイアウト確定後に実行
            card.schedule.Execute(() =>
            {
                Rect start = card.worldBound;

                _deck.Remove(card);
                _overlay.Add(card);

                card.style.position = Position.Absolute;
                card.style.left = start.x;
                card.style.top = start.y;
                card.style.width = start.width;
                card.style.height = start.height;

                Rect stackWorld = _stack.worldBound;

                Vector2 end = new Vector2(
                    stackWorld.x,
                    stackWorld.y + _stack.childCount * (start.height + 8)
                );

                const float duration = 0.3f;
                float elapsed = 0f;

                // 毎フレーム補間
                IVisualElementScheduledItem anim = null;
                anim = card.schedule.Execute(() =>
                {
                    elapsed += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsed / duration);
                    t = Mathf.SmoothStep(0, 1, t);

                    card.style.left = Mathf.Lerp(start.x, end.x, t);
                    card.style.top = Mathf.Lerp(start.y, end.y, t);

                    if (t >= 1f)
                    {
                        anim.Pause();

                        _overlay.Remove(card);
                        _stack.Add(card);

                        card.style.position = Position.Relative;
                        card.style.left = StyleKeyword.Auto;
                        card.style.top = StyleKeyword.Auto;
                        card.style.width = StyleKeyword.Auto;
                        card.style.height = StyleKeyword.Auto;
                    }
                }).Every(16);
            });
        }

        private const string DECK_NAME = "deck";
        private const string STACK_NAME = "stack";

        private VisualElement _deck;
        private VisualElement _stack;
        private VisualElement _overlay;
        private readonly Dictionary<CardViewModel, UIElementCard> _cards = new();
    }
}