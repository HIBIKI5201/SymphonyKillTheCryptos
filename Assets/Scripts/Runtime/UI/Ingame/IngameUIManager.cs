using Cryptos.Runtime.Ingame.Entity;
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
        Task IInitializeAsync.InitializeTask { get; set; }

        private UIDocument _document;
        private DeckManager _deckManager;

        private UIElementDeck _deck;

        async Task IInitializeAsync.InitializeAsync()
        {
            _document = GetComponent<UIDocument>();

            VisualElement root = _document.rootVisualElement;
            _deck = root.Q<UIElementDeck>();

            await _deck.InitializeTask;

            _deckManager = await ServiceLocator.GetInstanceAsync<DeckManager>();
            _deckManager.OnAddCardInstance += _deck.AddCard;
            _deckManager.OnRemoveCardInstance += _deck.RemoveCard;
        }
    }
}