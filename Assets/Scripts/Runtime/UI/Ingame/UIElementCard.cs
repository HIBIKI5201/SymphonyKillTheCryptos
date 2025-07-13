using SymphonyFrameWork.Utility;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using Cryptos.Runtime.Entity.Ingame.Card;

namespace Cryptos.Runtime.UI.Ingame
{
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
        public void SetData(CardEntity instance)
        {
            instance.OnWordUpdated += OnWordUpdate;
            instance.OnProgressUpdate += OnProgressBarUpdate;

            //初期値を入れる
            OnWordUpdate(instance.CurrentWord, 0);
            OnProgressBarUpdate(0);
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
        private void OnWordUpdate(string word, int index)
        {
            string newText = $"<b><color=green>{word[..index]}</color></b>{word[index..]}";
            _wordLabel.text = newText;
        }

        /// <summary>
        ///     進捗率を更新する
        /// </summary>
        /// <param name="progress"></param>
        private void OnProgressBarUpdate(float progress)
        {
            progress = progress * 100;
            _progressBar.style.width = Length.Percent(progress);
        }
    }
}
