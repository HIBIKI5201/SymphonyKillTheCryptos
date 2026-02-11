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

        public IDeckEditorUI DeckEditor => _deckEditor;

        protected override async Task InitializeDocumentAsync(UIDocument document, VisualElement root)
        {
            VisualElement buttonContainer = root.Q<VisualElement>(BUTTONS_NAME);
           if (buttonContainer == null) { return; }

            _startButton = buttonContainer.Q<Button>(START_BUTTON_NAME);
            if (_startButton == null) { return; }

            _startButton.RegisterCallback<NavigationSubmitEvent>(HandleStartButtonPressed);
            _startButton.Focus(); // 初期状態はスタートボタンをフォーカスする。

            _deckEditor = root.Q<UIElementDeckEditor>();

            destroyCancellationToken.Register(() =>
            {
                _startButton.UnregisterCallback<NavigationSubmitEvent>(HandleStartButtonPressed);
            });

            void HandleStartButtonPressed(NavigationSubmitEvent evt)
            {
                _onStartButtonPressed?.Invoke();
            }
        }

        private const string BUTTONS_NAME = "buttons";
        private const string START_BUTTON_NAME = "start";

        private Button _startButton;

        private event Action _onStartButtonPressed;
        private UIElementDeckEditor _deckEditor;
    }
}
