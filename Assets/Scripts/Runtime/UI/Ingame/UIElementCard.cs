using Cryptos.Runtime.Presenter.Ingame.Card;
using SymphonyFrameWork.Utility;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Cryptos.Runtime.UI.Ingame
{
    /// <summary>
    ///     カードのUI要素
    /// </summary>
    [UxmlElement]
    public partial class UIElementCard : SymphonyVisualElement
    {
        public UIElementCard() : base("UIToolKit/UXML/Ingame/Card", InitializeType.PickModeIgnore) { }

        /// <summary>
        ///     データをセットする
        /// </summary>
        /// <param name="data"></param>
        public void SetData(CardViewModel instance)
        {
            instance.OnWordUpdated += HandleWordUpdate;
            instance.OnProgressUpdate += HandleProgressBarUpdate;

            //初期値を入れる
            HandleWordUpdate(instance.CurrentWord, 0);
            HandleProgressBarUpdate(0);

            _iconElement.style.backgroundImage = new(instance.Icon);
        }

        protected override Task Initialize_S(TemplateContainer container)
        {
            style.marginRight = SIDE_MARGIN;
            style.marginLeft = SIDE_MARGIN;

            _iconElement = container.Q<VisualElement>(ICON_NAME);
            _wordLabel = container.Q<Label>(WORD_LABEL_NAME);
            _progressBar = container.Q<VisualElement>(PROGRESS_BAR_NAME);

            return Task.CompletedTask;
        }

        private const string ICON_NAME = "icon";
        private const string WORD_LABEL_NAME = "word";
        private const string PROGRESS_BAR_NAME = "progress-bar";

        private const int SIDE_MARGIN = 10;

        private VisualElement _iconElement;
        private Label _wordLabel;
        private VisualElement _progressBar;

        /// <summary>
        ///     ワードを更新する
        /// </summary>
        /// <param name="word"></param>
        /// <param name="index"></param>
        private void HandleWordUpdate(string word, int index)
        {
            string newText = $"<b><color=green>{word[..index]}</color></b>{word[index..]}";
            _wordLabel.text = newText;
        }

        /// <summary>
        ///     進捗率を更新する
        /// </summary>
        /// <param name="progress"></param>
        private void HandleProgressBarUpdate(float progress)
        {
            progress = progress * 100;
            _progressBar.style.width = Length.Percent(progress);
        }
    }
}
