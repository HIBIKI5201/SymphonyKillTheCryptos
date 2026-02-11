using Cryptos.Runtime.Presenter.Ingame.Card;
using SymphonyFrameWork.Utility;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cryptos.Runtime.UI.Ingame.Card
{
    /// <summary>
    ///     カードのUI要素
    /// </summary>
    [UxmlElement]
    public partial class UIElementCard : VisualElementBase
    {
        public UIElementCard() : base("InGameCard", InitializeType.PickModeIgnore) { }

        public Vector2 GetSize() => new(150, 200);
            // 不明な問題で大きさが取得できないため一旦定数で代用。
            //new(_root.style.width.value.value, _root.style.height.value.value);
        
        /// <summary>
        ///     データをセットする。
        /// </summary>
        /// <param name="data"></param>
        public async ValueTask SetData(CardViewModel instance)
        {
            instance.OnWordUpdated += HandleWordUpdate;
            instance.OnProgressUpdate += HandleProgressBarUpdate;

            await InitializeTask;

            //初期値を入れる。
            HandleWordUpdate(instance.CurrentWord, 0);
            HandleProgressBarUpdate(0);

            _iconElement.style.backgroundImage = new(instance.Icon);
        }

        public ValueTask Rotate()
        {
            _root.AddToClassList(CARD_STACK_STYLE);
            return default;
        }

        protected override ValueTask Initialize_S(VisualElement root)
        {
            style.marginRight = SIDE_MARGIN;
            style.marginLeft = SIDE_MARGIN;

            _root = root.Q<VisualElement>(ROOT_NAME);
            _iconElement = root.Q<VisualElement>(ICON_NAME);
            _wordLabel = root.Q<Label>(WORD_LABEL_NAME);
            _progressBar = root.Q<VisualElement>(PROGRESS_BAR_NAME);

            return default;
        }

        private const string ROOT_NAME = "root";
        private const string ICON_NAME = "icon";
        private const string WORD_LABEL_NAME = "word";
        private const string PROGRESS_BAR_NAME = "progress-bar";

        private const string CARD_STACK_STYLE = "card-stack";

        private const int SIDE_MARGIN = 10;

        private VisualElement _root;
        private VisualElement _iconElement;
        private Label _wordLabel;
        private VisualElement _progressBar;

        private CancellationTokenSource _progressTaskToken;

        /// <summary>
        ///     ワードを更新する
        /// </summary>
        /// <param name="word"></param>
        /// <param name="index"></param>
        private async void HandleWordUpdate(string word, int index)
        {
            await InitializeTask;
            string newText = $"<b><color=green>{word[..index]}</color></b>{word[index..]}";
            _wordLabel.text = newText;
        }

        /// <summary>
        ///     進捗率を更新する
        /// </summary>
        /// <param name="progress"></param>
        private async void HandleProgressBarUpdate(float progress)
        {
            await InitializeTask;

            if (_progressTaskToken != null
                && !_progressTaskToken.IsCancellationRequested)
            {
                _progressTaskToken.Cancel();
                _progressTaskToken.Dispose();
                _progressTaskToken = null;
            }

            Length length = _progressBar.style.width.value;
            progress = progress * 100;

            _progressTaskToken = new();
            try
            {
                await SymphonyTween.Tweening(length.value,
                    n => _progressBar.style.width = Length.Percent(n),
                    progress,
                    0.2f, // 仮の値。
                    token: _progressTaskToken.Token);
            }
            catch
            {
                _progressBar.style.width = Length.Percent(progress);
            }
        }
    }
}
