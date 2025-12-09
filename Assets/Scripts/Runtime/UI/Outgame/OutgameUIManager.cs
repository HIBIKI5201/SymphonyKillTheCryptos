using Cryptos.Runtime.UI.Basis;
using System;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Cryptos.Runtime.UI.Outgame
{
    public class OutgameUIManager : UIManagerBase
    {
        /// <summary> スタートボタンが押された時 </summary>
        public event Action OnPressedStartButton
        {
            add  => _startButton.clicked += value;
            remove => _startButton.clicked -= value;
        }

        protected override async Task InitializeDocumentAsync(UIDocument document, VisualElement root)
        {
            VisualElement buttonContainer = root.Q<VisualElement>(BUTTONS_NAME);
            _startButton = buttonContainer.Q<Button>(START_BUTTON_NAME);

            if (_startButton == null) { return; }

            _startButton.Focus(); // 初期状態はスタートボタンをフォーカスする。
        }

        private const string BUTTONS_NAME = "buttons";
        private const string START_BUTTON_NAME = "start";

        private Button _startButton;
    }
}
