using Cryptos.Runtime.Presenter.Ingame.System;
using System;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Cryptos.Runtime.UI.Ingame.Manager
{
    [UxmlElement]
    public partial class UIElementResultWindow : VisualElementBase
    {
        public event Action OnReturnButtonClicked;

        public UIElementResultWindow() : base(UXML_PATH) { }

        protected override async ValueTask Initialize_S(VisualElement root)
        {
            _resultTitle = root.Q<Label>(RESULT_TITLE_NAME);
            _scoreWaveText = root.Q<Label>(SCORE_TEXT_WAVE_NAME);
            _scoreTimeText = root.Q<Label>(SCORE_TEXT_TIME_NAME);
            _scoreSkillText = root.Q<Label>(SCORE_TEXT_SKILL_NAME);
            _returnButton = root.Q<Button>(RETURN_BUTTON_NAME);

            _returnButton.clicked += () => OnReturnButtonClicked?.Invoke();
            await Task.CompletedTask;
        }

        public void SetResult(ResultViewModel vm)
        {
            _scoreWaveText.text = $"wave: {vm.Wave}";
            _scoreTimeText.text = $"time: {vm.Time}";
            _scoreSkillText.text = $"Skill Point: {vm.SkillPoint}";
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
        private const string SCORE_TEXT_WAVE_NAME = "score-text-wave";
        private const string SCORE_TEXT_TIME_NAME = "score-text-time";
        private const string SCORE_TEXT_SKILL_NAME = "score-text-skill";
        private const string RETURN_BUTTON_NAME = "return-button";

        private Label _resultTitle;
        private Label _scoreWaveText;
        private Label _scoreTimeText;
        private Label _scoreSkillText;
        private Button _returnButton;
    }
}
