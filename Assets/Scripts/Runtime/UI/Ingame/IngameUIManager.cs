using Cryptos.Runtime.Presenter.System;
using Cryptos.Runtime.Presenter.Ingame.Card;
using SymphonyFrameWork;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Cryptos.Runtime.Framework;
using SymphonyFrameWork.System;
using System.Threading;
using System;

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

        public void OpenLevelUpgradeWindow(Span<LevelUpgradeNodeViewModel> nodes)
        {
            _levelUpgrade.OpenWindow(nodes);
        }

        public void CloseLevelUpgradeWindow()
        {
            _levelUpgrade.CloseWindow();
        }

        public bool TryGetSelectedLevelUpgradeNode(out LevelUpgradeNodeViewModel nodeVM)
        {
            UIElementLevelUpgradeNode node = _levelUpgrade.GetSelectedLevelUpgrade();

            bool isNull = node == null;
            nodeVM = isNull ? default : node.NodeViewModel;

            return !isNull;
        }

        public void OnInutChar(char c)
        {
            _levelUpgrade.InputChar(c);
        }

        Task IInitializeAsync.InitializeTask { get; set; }

        private UIDocument _document;

        private UIElementDeck _deck;
        private UIElementLevelUpgradeWindow _levelUpgrade;

        private InputBuffer _inputBuffer;

        async Task IInitializeAsync.InitializeAsync()
        {
            _document = GetComponent<UIDocument>();

            VisualElement root = _document.rootVisualElement;
            _deck = root.Q<UIElementDeck>();
            _levelUpgrade = root.Q<UIElementLevelUpgradeWindow>();  

            await _deck.InitializeTask;
            await _levelUpgrade.InitializeTask;

            _inputBuffer = await ServiceLocator.GetInstanceAsync<InputBuffer>();

            _levelUpgrade.CloseWindow();
        }
    }
}