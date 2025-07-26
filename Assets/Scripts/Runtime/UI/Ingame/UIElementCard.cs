using SymphonyFrameWork.Utility;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using Cryptos.Runtime.Entity.Ingame.Card;

namespace Cryptos.Runtime.UI.Ingame
{
    /// <summary>
    ///     カードのUI要素
    /// </summary>
    [UxmlElement]
    public partial class UIElementCard : SymphonyVisualElement
    {
        public UIElementCard() : base("UIToolKit/UXML/Ingame/Card", InitializeType.PickModeIgnore) { }

        private const int SIDE_MARGIN = 10;

        private Label _wordLabel;
        private VisualElement _progressBar;

        /// <summary>
        ///     データをセットする
        /// </summary>
        /// <param name="data"></param>
        public void SetData(WordEntity instance)
        {
            instance.OnWordUpdated += HandleWordUpdate;
            instance.OnProgressUpdate += HandleProgressBarUpdate;

            //初期値を入れる
            HandleWordUpdate(instance.CurrentWord, 0);
            HandleProgressBarUpdate(0);
        }

        protected override Task Initialize_S(TemplateContainer container)
        {
            style.marginRight = SIDE_MARGIN;
            style.marginLeft = SIDE_MARGIN;

            _wordLabel = container.Q<Label>("word");
            _progressBar = container.Q<VisualElement>("progress-bar");

            return Task.CompletedTask;
        }

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
