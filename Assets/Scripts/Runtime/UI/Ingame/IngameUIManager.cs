using Cryptos.Runtime.UseCase.Ingame.Card;
using SymphonyFrameWork;
using SymphonyFrameWork.System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cryptos.Runtime.UI.Ingame
{
    /// <summary>
    ///     インゲームのUIを管理する
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class IngameUIManager : MonoBehaviour, IInitializeAsync
    {
        public void Init(CardUseCase cardUseCase)
        {
            cardUseCase.OnCardAddedToDeck += _deck.HandleAddCard;
            cardUseCase.OnCardRemovedFromDeck += _deck.HandleRemoveCard;
        }

        Task IInitializeAsync.InitializeTask { get; set; }

        private UIDocument _document;

        private UIElementDeck _deck;

        async Task IInitializeAsync.InitializeAsync()
        {
            _document = GetComponent<UIDocument>();

            VisualElement root = _document.rootVisualElement;
            _deck = root.Q<UIElementDeck>();

            await _deck.InitializeTask;
        }
    }
}