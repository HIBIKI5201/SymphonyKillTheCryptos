using Cryptos.Runtime.Presenter;
using Cryptos.Runtime.Presenter.System;
using SymphonyFrameWork.Utility;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

namespace Cryptos.Runtime.UI.Ingame
{
    [UxmlElement]
    public partial class UIElementLevelUpgradeWindow : SymphonyVisualElement
    {
        public UIElementLevelUpgradeWindow() : base("UIToolKit/UXML/Ingame/LevelUpgradeWindow")
        {

        }

        public void InputChar(char c)
        {
            foreach (var node in _nodes)
            {
                // 非表示なら更新しない
                if (node.style.display == DisplayStyle.None) return;

                node.OnInputChar(c);
            }
        }

        public void OnenWindow(Span<LevelUpgradeNodeViewModel> nodeVMs)
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

        public void CloseWindow()
        {
            if (_nodes != null) // ノードを破棄する
            {
                foreach (var node in _nodes)
                {
                    _nodeContainer.Remove(node);
                }

                _nodes = null;
            }

            style.display = DisplayStyle.None;
        }

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
