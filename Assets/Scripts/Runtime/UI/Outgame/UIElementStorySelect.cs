using System;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UIElements;

namespace Cryptos.Runtime.UI
{
    [UxmlElement]
    public partial class UIElementStorySelect : VisualElementBase
    {
        public UIElementStorySelect() : base("StorySelect") { }

        public event Action<int> OnButtonClicked;

        public void Show()
        {
            style.display = DisplayStyle.Flex;
            _buttons[0].Focus();
        }

        public void Close()
        {
            style.display = DisplayStyle.None;
        }

        protected override async ValueTask Initialize_S(VisualElement root)
        {
            VisualElement container = root.Q<VisualElement>(BUTTON_CONTAINER_NAME);
            UQueryBuilder<Button> query = container.Query<Button>();
            _buttons = query.ToList().ToArray();
            for (int i = 0; i < _buttons.Length; i++)
            {
                int index = i;
                _buttons[i].clicked += () => OnButtonClicked?.Invoke(index);
            }

            if (EventSystem.current.TryGetComponent(out InputSystemUIInputModule module))
            {
                _uiInputModule = module;
                module.cancel.action.started += OnNavigationCancel;
            }
        }

        private const string BUTTON_CONTAINER_NAME = "button-container";

        private Button[] _buttons;
        private InputSystemUIInputModule _uiInputModule;

        private void OnNavigationCancel(InputAction.CallbackContext context)
        {
            Close();
        }

    }
}
