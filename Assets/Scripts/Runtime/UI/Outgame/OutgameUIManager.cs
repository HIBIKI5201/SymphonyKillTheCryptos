using Cryptos.Runtime.Presenter.OutGame;
using Cryptos.Runtime.UI.Basis;
using Cryptos.Runtime.UI.Outgame.Deck;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cryptos.Runtime.UI.Outgame
{
    /// <summary>
    ///     アウトゲームのUIを管理するクラスである。
    /// </summary>
    public class OutgameUIManager : UIManagerBase
    {
        /// <summary> スタートボタンが押された時 </summary>
        public event Action OnPressedStartButton
        {
            add => _onStartButtonPressed += value;
            remove => _onStartButtonPressed -= value;
        }

        public event Action<int> OnStoryButtonClicked
        {
            add => _storySelect.OnButtonClicked += value;
            remove => _storySelect.OnButtonClicked -= value;
        }

        public IDeckEditorUI DeckEditor => _deckEditor;

        protected override async ValueTask InitializeDocumentAsync(UIDocument document, VisualElement root)
        {
            _deckEditor = root.Q<UIElementDeckEditor>();
            _storySelect = root.Q<UIElementStorySelect>();

            VisualElement buttonContainer = root.Q<VisualElement>(BUTTONS_NAME);
            _buttonContainer = buttonContainer;
           if (buttonContainer == null) { return; }

            _startButton = buttonContainer.Q<Button>(START_BUTTON_NAME);
            if (_startButton == null) { return; }

            _startButton.RegisterCallback<NavigationSubmitEvent>(HandleStartButtonPressed);
            _startButton.Focus();
            
            _editButton = buttonContainer.Q<Button>(EDIT_BUTTON_NAME);
            _editButton.clicked += _deckEditor.Show;
            _editButton.clicked += ButtonsHide;
            _deckEditor.OnSaveButtonClicked += ButtonsShow;

            destroyCancellationToken.Register(() =>
            {
                _startButton.UnregisterCallback<NavigationSubmitEvent>(HandleStartButtonPressed);
            });

            void HandleStartButtonPressed(NavigationSubmitEvent evt)
            {
                _onStartButtonPressed?.Invoke();
            }

            _storyButton = root.Q<Button>(STORY_BUTTON_NAME);
            _storyButton.clicked += _storySelect.Show;

            _quitButton = root.Q<Button>(QUIT_BUTTON_NAME);
            _quitButton.clicked += Application.Quit;
        }

        private const string BUTTONS_NAME = "buttons";
        private const string START_BUTTON_NAME = "start";
        private const string EDIT_BUTTON_NAME = "edit";
        private const string STORY_BUTTON_NAME = "story";
        private const string QUIT_BUTTON_NAME = "quit";

        private VisualElement _buttonContainer;
        private Button _startButton;
        private Button _editButton;
        private Button _storyButton;
        private Button _quitButton;

        private event Action _onStartButtonPressed;
        private UIElementDeckEditor _deckEditor;
        private UIElementStorySelect _storySelect;

        private void ButtonsShow()
        {
            _buttonContainer.style.visibility = Visibility.Visible;
        }
        private void ButtonsHide()
        {
            _buttonContainer.style.visibility = Visibility.Hidden;
        }
    }
}
