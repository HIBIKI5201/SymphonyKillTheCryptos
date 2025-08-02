using Cryptos.Runtime.Presenter.Ingame.Card;
using SymphonyFrameWork;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cryptos.Runtime.UI.Ingame
{
    /// <summary>
    ///     インゲームのUIを管理する
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class IngameUIManager : MonoBehaviour, ICardUIManager, IInitializeAsync
    {
        public void HanldeAddCard(CardViewModel instance)
        {
            _deck.HandleAddCard(instance);
        }

        public void HandleRemoveCard(CardViewModel instance)
        {
            _deck.HandleRemoveCard(instance);
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