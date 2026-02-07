using System;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Cryptos.Runtime.UI.Ingame.Manager
{
    public partial class UIElementResultWindow : VisualElementBase
    {
        public event Action OnReturnButtonClicked;

        public UIElementResultWindow() : base(UXML_PATH) { }

        protected override async ValueTask Initialize_S(VisualElement root)
        {
            _resultTitle = root.Q<Label>(RESULT_TITLE_NAME);
            _scoreText = root.Q<Label>(SCORE_TEXT_NAME);
            _returnButton = root.Q<Button>(RETURN_BUTTON_NAME);

            _returnButton.clicked += () => OnReturnButtonClicked?.Invoke();
            await Task.CompletedTask;
        }

        public void SetResult(string title, int score)
        {
            _resultTitle.text = title;
            _scoreText.text = $"Score: {score}";
        }

        public void Open()
        {
            style.display = DisplayStyle.Flex;
            _returnButton.Focus();
        }

        public void Close()
        {
            style.display = DisplayStyle.None;
        }

        private const string UXML_PATH = "ResultWindow";
        private const string RESULT_TITLE_NAME = "result-title";
        private const string SCORE_TEXT_NAME = "score-text";
        private const string RETURN_BUTTON_NAME = "return-button";

        private Label _resultTitle;
        private Label _scoreText;
        private Button _returnButton;
    }
}
