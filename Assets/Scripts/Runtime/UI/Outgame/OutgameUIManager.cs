using Cryptos.Runtime.UI.Basis;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cryptos.Runtime.UI.Outgame
{
    public class OutgameUIManager : UIManagerBase
    {
        /// <summary> スタートボタンが押された時 </summary>
        public event Action OnPressedStartButton
        {
            add => _onStartButtonPressed += value;
            remove => _onStartButtonPressed -= value;
        }

        protected override async Task InitializeDocumentAsync(UIDocument document, VisualElement root)
        {
            VisualElement buttonContainer = root.Q<VisualElement>(BUTTONS_NAME);
           if (buttonContainer == null) { return; }

            _startButton = buttonContainer.Q<Button>(START_BUTTON_NAME);

            if (_startButton == null) { return; }

            _startButton.RegisterCallback<NavigationSubmitEvent>(HandleStartButtonPressed);
            _startButton.Focus(); // 初期状態はスタートボタンをフォーカスする。

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
    }
}
