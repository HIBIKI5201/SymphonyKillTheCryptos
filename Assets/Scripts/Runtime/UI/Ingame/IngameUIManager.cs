using Cryptos.Runtime.Presenter.System;
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
        public void AddCard(CardViewModel instance)
        {
            _deck.HandleAddCard(instance);
        }

        public void RemoveCard(CardViewModel instance)
        {
            _deck.HandleRemoveCard(instance);
        }

        public void OpenLevelUpgradeWindow(LevelUpgradeNodeViewModel[] nodes)
        {
            _levelUpgrade.OnenWindow(nodes);
        }

        Task IInitializeAsync.InitializeTask { get; set; }

        private UIDocument _document;

        private UIElementDeck _deck;
        private UIElementLevelUpgradeWindow _levelUpgrade;

        async Task IInitializeAsync.InitializeAsync()
        {
            _document = GetComponent<UIDocument>();

            VisualElement root = _document.rootVisualElement;
            _deck = root.Q<UIElementDeck>();
            _levelUpgrade = root.Q<UIElementLevelUpgradeWindow>();  

            await _deck.InitializeTask;
            await _levelUpgrade.InitializeTask;

            _levelUpgrade.CloseWindow();
        }
    }
}