using Cryptos.Runtime.Presenter;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cryptos.Runtime.UI
{
    [UxmlElement]
    public partial class StoryMessageWindow : VisualElementBase
    {
        public StoryMessageWindow():base("StoryMessageWindow") { }

        public void BindViewModel(StoryMessageViewModel viewModel)
        {
            _vm = viewModel;
            viewModel.OnMessageReceived += MessageReceiveHandler;
        }

        protected override async ValueTask Initialize_S(VisualElement root)
        {
            _characterName = root.Q<Label>(CHACACTER_NAME_NAME);
            _mainText = root.Q<Label>(MAIN_TEXT_NAME);

            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        private const string CHACACTER_NAME_NAME = "name";
        private const string MAIN_TEXT_NAME = "text";

        private StoryMessageViewModel _vm;
        private Label _characterName;
        private Label _mainText;

        private void OnDetachFromPanel(DetachFromPanelEvent evt)
        {
            if (_vm == default) { return; }

            _vm.OnMessageReceived -= MessageReceiveHandler;
        }
                                                                                                                                                                                                                                                                                                                 
        private void MessageReceiveHandler(string name, string text)
        {
            _characterName.text = name;
            _mainText.text = text;
        }
    }
}
