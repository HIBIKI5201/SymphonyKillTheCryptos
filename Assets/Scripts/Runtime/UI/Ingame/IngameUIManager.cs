using Cryptos.Runtime.Framework;
using Cryptos.Runtime.Presenter.Ingame.Card;
using Cryptos.Runtime.Presenter.System;
using SymphonyFrameWork;
using SymphonyFrameWork.System;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cryptos.Runtime.UI.Ingame
{
    /// <summary>
    ///     インゲームのUIを管理します。
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class IngameUIManager : MonoBehaviour, ICardUIManager, IInitializeAsync
    {
        /// <summary>
        /// カードをUIに追加します。
        /// </summary>
        /// <param name="instance">追加するカードのビューモデル。</param>
        public void AddCard(CardViewModel instance)
        {
            _deck.HandleAddCard(instance);
        }

        /// <summary>
        /// カードをUIから削除します。
        /// </summary>
        /// <param name="instance">削除するカードのビューモデル。</param>
        public void RemoveCard(CardViewModel instance)
        {
            _deck.HandleRemoveCard(instance);
        }

        /// <summary>
        /// レベルアップウィンドウを開きます。
        /// </summary>
        /// <param name="nodes">表示するノード。</param>
        public void OpenLevelUpgradeWindow(Span<LevelUpgradeNodeViewModel> nodes)
        {
            _levelUpgrade.OpenWindow(nodes);
        }

        /// <summary>
        /// レベルアップウィンドウを閉じます。
        /// </summary>
        public void CloseLevelUpgradeWindow()
        {
            _levelUpgrade.CloseWindow();
        }

        /// <summary>
        /// 選択されたレベルアップノードを取得しようとします。
        /// </summary>
        /// <param name="nodeVM">選択されたノードのビューモデル。</param>
        /// <returns>ノードが選択されていればtrue、そうでなければfalse。</returns>
        public bool TryGetSelectedLevelUpgradeNode(out LevelUpgradeNodeViewModel nodeVM)
        {
            UIElementLevelUpgradeNode node = _levelUpgrade.GetSelectedLevelUpgrade();

            bool isNull = node == null;
            nodeVM = isNull ? default : node.NodeViewModel;

            return !isNull;
        }

        /// <summary>
        /// 文字入力を処理します。
        /// </summary>
        /// <param name="c">入力された文字。</param>
        public void OnInputChar(char c)
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