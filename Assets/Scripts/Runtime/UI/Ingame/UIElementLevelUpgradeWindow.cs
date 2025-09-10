using Cryptos.Runtime.Presenter;
using Cryptos.Runtime.Presenter.System;
using SymphonyFrameWork.Utility;
using System.Threading.Tasks;
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

        public void OnenWindow(LevelUpgradeNodeViewModel[] nodes)
        {
            for(int i = 0; i < _nodes.Length; i++)
            {
                if(i < nodes.Length)
                {
                    LevelUpgradeNodeViewModel node = nodes[i];

                    _nodes[i].SetData(node.Texture, node.NodeName, node.Description);
                    _nodes[i].style.display = DisplayStyle.Flex;
                }
                else // 入力以上のノードは非表示にする
                {
                    _nodes[i].style.display = DisplayStyle.None;
                }
            }

            style.display = DisplayStyle.Flex;
        }

        public void CloseWindow()
        {
            // とりあえず非表示にするだけ
            style.display = DisplayStyle.None;
        }

        protected override Task Initialize_S(TemplateContainer container)
        {
            _nodes = new UIElementLevelUpgradeNode[NODE_MAX];
            VisualElement nodeContainer = container.Q(NODE_CONTAINER);

            for (int i = 0; i < NODE_MAX; i++)
            {
                UIElementLevelUpgradeNode node = new();
                nodeContainer.Add(node);

                _nodes[i] = node;
            }

            return Task.CompletedTask;
        }

        private const int NODE_MAX = 3;
        private const string NODE_CONTAINER = "container";

        private UIElementLevelUpgradeNode[] _nodes;
    }
}
