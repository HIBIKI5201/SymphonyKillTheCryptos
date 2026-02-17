using Cryptos.Runtime.UI.Basis;
using System;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UIElements;

namespace Cryptos.Runtime.UI.Outgame.Story
{
    public class StoryUIManager : UIManagerBase
    {
        public event Action OnNextClicked;
        public StoryMessageWindow MessageWindow => _messageWindow;

        protected override async ValueTask InitializeDocumentAsync(UIDocument document, VisualElement root)
        {
            _messageWindow = root.Q<StoryMessageWindow>();

            if (EventSystem.current.TryGetComponent(out InputSystemUIInputModule module))
            {
                _uiModule = module;
                module.submit.action.started += NextClicked;
            }

            await _messageWindow.InitializeTask;
        }

        private InputSystemUIInputModule _uiModule;
        private StoryMessageWindow _messageWindow;

        private void OnDestroy()
        {
            if (_uiModule == null) { return; }

            _uiModule.submit.action.started -= NextClicked;
        }

        private void NextClicked(InputAction.CallbackContext context)
        {
            OnNextClicked?.Invoke();
        }
    }
}
