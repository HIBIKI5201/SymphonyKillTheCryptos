using Cryptos.Runtime.Presenter.Ingame.Card;
using SymphonyFrameWork.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cryptos.Runtime.UI.Ingame.Card
{
    /// <summary>
    ///     デッキのUI
    /// </summary>
    [UxmlElement]
    public partial class UIElementDeck : VisualElementBase
    {
        public UIElementDeck() : base("Hand") { }

        protected override async ValueTask Initialize_S(VisualElement root)
        {
            _deck = root.Q<VisualElement>(DECK_NAME);
            _stack = root.Q<VisualElement>(STACK_NAME);

            VisualElement overlay = new();
            overlay.style.position = Position.Absolute;
            overlay.style.flexGrow = 1;
            overlay.style.width = Length.Percent(100);
            overlay.style.height = Length.Percent(100);
            root.Add(overlay);
            _overlay = overlay;

            TaskCompletionSource<bool> cacheRectTask = new();

            _overlay.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                _cachedDeckRect = _deck.worldBound;
                _cachedStackRect = _stack.worldBound;
                cacheRectTask.TrySetResult(true);
            });

            await cacheRectTask.Task;
        }

        /// <summary>
        ///     カードをデッキに追加する
        /// </summary>
        /// <param name="instance"></param>
        public async void HandleAddCard(CardViewModel instance)
        {
            UIElementCard card = new();
            _ = card.SetData(instance);

            _overlay.Add(card);
            _cards.Add(instance, card);

            await InitializeTask;

            _overlay.schedule.Execute(() =>
            {
                card.style.position = Position.Absolute;
                _cardModes[card] = Mode.Deck;
                LayoutDeckCards();
            });
        }

        /// <summary>
        ///     カードをデッキから削除する
        /// </summary>
        /// <param name="instance"></param>
        public async void HandleRemoveCard(CardViewModel instance)
        {
            if (_cards.TryGetValue(instance, out UIElementCard card))
            {
                if (_moveTask.TryGetValue(instance, out ValueTask task))
                {
                    await task;
                }

                _cards.Remove(instance);
                _cardModes.Remove(card);

                _overlay.Remove(card);


                LayoutDeckCards();
                ReLayoutStackCards();
            }
        }

        public async void MoveCardToStack(CardViewModel instance)
        {
            await InitializeTask;

            ValueTask valueTask = MoveCardToStackAsync(instance);
            _moveTask.TryAdd(instance, valueTask);
            await valueTask;
            _moveTask.Remove(instance);
        }

        public async ValueTask MoveCardToStackAsync(CardViewModel instance)
        {
            await InitializeTask;

            if (!_cards.TryGetValue(instance, out UIElementCard card)) { return; }
            _cardModes[card] = Mode.Stack;
            LayoutDeckCards();

            TaskCompletionSource<bool> taskCompletion = new();

            card.schedule.Execute(async () =>
            {
                Rect start = card.worldBound;

                card.style.position = Position.Absolute;
                card.style.left = start.x;
                card.style.top = start.y;
                card.style.width = start.width;
                card.style.height = start.height;

                Rect stackWorld = _cachedStackRect;
                float assumedCardWidth = card.resolvedStyle.width > 0 ? card.resolvedStyle.width : 100;
                float assumedCardHeight = card.resolvedStyle.height > 0 ? card.resolvedStyle.height : 150;
                const float stackSpacing = 8f; // カード間の間隔の仮数値。

                Vector2 end = new Vector2(
                    stackWorld.x + (stackWorld.width - assumedCardWidth) / 2f, // Stackの中央に配置。
                    stackWorld.y + _cachedStackRect.height - ((_cards.Values.Count(c => _cardModes.TryGetValue(c, out Mode m) && m == Mode.Stack)) * (assumedCardHeight + stackSpacing))
                );

                const float duration = 0.3f;

                await SymphonyTween.Tweening(start.x,
                    n => card.style.left = n,
                    end.x, duration);
                await SymphonyTween.Tweening(start.y,
                    n => card.style.top = n,
                    end.y, duration);

                card.style.left = end.x;
                card.style.top = end.y;
                card.style.width = assumedCardWidth;
                card.style.height = assumedCardHeight;
                taskCompletion.SetResult(true);
                ReLayoutStackCards();
            });

            await card.Rotate();
            await taskCompletion.Task;
        }

        private const string DECK_NAME = "deck";
        private const string STACK_NAME = "stack";

        private VisualElement _deck;
        private VisualElement _stack;
        private VisualElement _overlay;
        private Rect _cachedDeckRect;
        private Rect _cachedStackRect;
        private readonly Dictionary<CardViewModel, UIElementCard> _cards = new();
        private readonly Dictionary<CardViewModel, ValueTask> _moveTask = new();
        private readonly Dictionary<UIElementCard, Mode> _cardModes = new();

        private enum Mode
        {
            Deck,
            Stack
        }

        private async void LayoutDeckCards()
        {
            await InitializeTask;

            UIElementCard[] cardsToLayout =
                _cards.Values
                .Where(card => _cardModes.TryGetValue(card, out Mode mode) && mode == Mode.Deck)
                .ToArray();

            if (cardsToLayout.Length == 0) { return; }


            Rect deckWorld = _cachedDeckRect;
            Rect overlayWorld = _overlay.worldBound;
            Debug.Log($"overlay {overlayWorld}, deck {deckWorld}");

            float deckRelativeX = deckWorld.x - overlayWorld.x;
            float deckRelativeY = deckWorld.y - overlayWorld.y;

            UIElementCard firstCard = cardsToLayout[0];
            await WaitGeometoryAsync(firstCard);
            await firstCard.InitializeTask;
            float cardVisualWidth = GetCardTotalWidth(firstCard);
            float cardHeight = firstCard.GetSize().y;

            float spacing = 30f; // 仮の値。

            float totalCardsWidth =
                (cardsToLayout.Length * cardVisualWidth)
                + ((cardsToLayout.Length - 1) * spacing);

            float startX = deckRelativeX + (deckWorld.width - totalCardsWidth) * 0.5f;
            float cardY = deckRelativeY + (deckWorld.height - cardHeight) * 0.5f;

            float currentX = startX;

            foreach (var card in cardsToLayout)
            {
                MoveCard(card, new Vector2(currentX, cardY));

                currentX += cardVisualWidth + spacing;
            }
        }

        private async void ReLayoutStackCards()
        {
            await InitializeTask;

            UIElementCard[] stackCardsToLayout =
                _cards.Values
                .Where(card => _cardModes.TryGetValue(card, out Mode mode) && mode == Mode.Stack)
                .ToArray();

            if (stackCardsToLayout.Length == 0) { return; }

            Rect stackWorld = _cachedStackRect;
            Rect overlayWorld = _overlay.worldBound;
            Debug.Log($"ReLayoutStackCards: Overlay World: {overlayWorld}");

            if (overlayWorld.width <= 0 || overlayWorld.height <= 0)
            {
                Debug.LogWarning($"ReLayoutStackCards: _overlay.worldBound is 0. Scheduling retry.");
                _overlay.schedule.Execute(() => ReLayoutStackCards());
                return;
            }

            float stackRelativeX = stackWorld.x - overlayWorld.x;
            float stackRelativeY = stackWorld.y - overlayWorld.y;

            const float stackSpacing = 8f;

            float cardWidth = 0, cardHeight = 0;
            if (0 < stackCardsToLayout.Length)
            {
                UIElementCard firstCard = stackCardsToLayout.First();
                await WaitGeometoryAsync(firstCard);
                await firstCard.InitializeTask;
                Vector2 size = firstCard.GetSize();
                // 横に向けるためサイズが逆転する。
                cardWidth = size.y;
                cardHeight = size.x;         
            }

            // スタックの底から積み上げる。
            int cardCountInStack = stackCardsToLayout.Length;

            // 各カードのY座標を計算
            for (int i = 0; i < cardCountInStack; i++)
            {
                UIElementCard card = stackCardsToLayout[cardCountInStack - 1 - i];

                float targetY = stackRelativeY + stackWorld.height - ((i + 1) * (cardHeight + stackSpacing));

                Debug.Log($"ReLayoutStackCards: Card {(_cards.FirstOrDefault(x => x.Value == card).Key.CurrentWord ?? "N/A")} - Calculated Left: {stackRelativeX + (stackWorld.width - cardWidth) / 2f}, Top: {targetY}, Width: {cardWidth}, Height: {cardHeight}");

                Vector2 pos = new(stackRelativeX + (stackWorld.width - cardWidth) / 2f, targetY);
                MoveCard(card, pos);

                card.style.width = cardWidth;
                card.style.height = cardHeight;
            }
        }

        private void MoveCard(UIElementCard card, Vector2 pos)
        {
            IStyle style = card.style;
            style.left = pos.x;
            style.top = pos.y;

            Debug.Log($"[card] {pos}");
        }

        private float GetCardTotalWidth(UIElementCard ve)
        {
            IStyle style = ve.style;
            return
                style.marginLeft.value.value +
                ve.GetSize().x +
                style.marginRight.value.value;
        }

        private async ValueTask WaitGeometoryAsync(VisualElement element)
        {
            if (!float.IsNaN(element.resolvedStyle.width)) { return; }

            TaskCompletionSource<bool> tcs = new();

            element.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            await tcs.Task;

            void OnGeometryChanged(GeometryChangedEvent evt)
            {
                element.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
                tcs.TrySetResult(true);
            }
        }
    }
}