using Cryptos.Runtime.Presenter.System;
using SymphonyFrameWork.Utility;
using System;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Cryptos.Runtime.UI.Ingame.LevelUp
{
    /// <summary>
    /// レベルアップ時の選択ウィンドウのUI要素。
    /// </summary>
    [UxmlElement]
    public partial class UIElementLevelUpgradeWindow : SymphonyVisualElement
    {
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public UIElementLevelUpgradeWindow() : base("UIToolKit/UXML/InGame/LevelUpgradeWindow")
        {

        }

        /// <summary>
        /// 文字入力を処理します。
        /// </summary>
        /// <param name="c">入力された文字。</param>
        public void InputChar(char c)
        {
            foreach (var node in _nodes)
            {
                // 非表示なら更新しない。
                if (node.style.display == DisplayStyle.None) return;

                node.OnInputChar(c);
            }
        }

        /// <summary>
        /// ウィンドウを開き、レベルアップノードを表示します。
        /// </summary>
        /// <param name="nodeVMs">表示するノードのビューモデル。</param>
        public void OpenWindow(Span<LevelUpgradeNodeViewModel> nodeVMs)
        {
            _nodes = new UIElementLevelUpgradeNode[NODE_MAX];

            for (int i = 0; i < NODE_MAX; i++)
            {
                if (nodeVMs.Length <= i) break;

                UIElementLevelUpgradeNode node = new();
                LevelUpgradeNodeViewModel nodeVM = nodeVMs[i];

                node.SetData(nodeVM);

                _nodeContainer.Add(node);
                _nodes[i] = node;
            }

            style.display = DisplayStyle.Flex;
        }

        /// <summary>
        /// ウィンドウを閉じます。
        /// </summary>
        public void CloseWindow()
        {
            if (_nodes != null) // ノードを破棄する。
            {
                foreach (var node in _nodes)
                {
                    _nodeContainer.Remove(node);
                }

                _nodes = null;
            }

            style.display = DisplayStyle.None;
        }

        /// <summary>
        /// 選択されたレベルアップノードを取得します。
        /// </summary>
        /// <returns>選択されたノード。選択されていない場合はnull。</returns>
        public UIElementLevelUpgradeNode GetSelectedLevelUpgrade()
        {
            foreach (var node in _nodes)
            {
                if (node.IsSelected)
                {
                    return node;
                }
            }

            return null;
        }

        protected override Task Initialize_S(TemplateContainer container)
        {
            _nodeContainer = container.Q(NODE_CONTAINER);

            return Task.CompletedTask;
        }

        private const int NODE_MAX = 3;
        private const string NODE_CONTAINER = "container";
        private VisualElement _nodeContainer;

        private UIElementLevelUpgradeNode[] _nodes;
    }
}
