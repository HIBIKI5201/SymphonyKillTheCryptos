using Cryptos.Runtime.Presenter.Ingame.Word;
using Cryptos.Runtime.Presenter.System;
using SymphonyFrameWork.Utility;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Cryptos.Runtime.UI.Ingame.LevelUp
{
    /// <summary>
    /// レベルアップノードのUI要素。
    /// </summary>
    [UxmlElement]
    public partial class UIElementLevelUpgradeNode : SymphonyVisualElement
    {
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public UIElementLevelUpgradeNode() : base("UIToolKit/UXML/InGame/LevelUpgradeNode", 0)
        {
        }

        /// <summary>
        /// このノードが選択されたかどうか。
        /// </summary>
        public bool IsSelected => _isSelected;
        /// <summary>
        /// このノードのビューモデル。
        /// </summary>
        public LevelUpgradeNodeViewModel NodeViewModel => _nodeViewModel;

        /// <summary>
        /// 文字入力を処理します。
        /// </summary>
        /// <param name="c">入力された文字。</param>
        public void OnInputChar(char c)
        {
            _wordEntity.InputChar(c);
        }

        /// <summary>
        /// このノードにデータを設定します。
        /// </summary>
        /// <param name="vm">設定するデータを含むビューモデル。</param>
        public void SetData(LevelUpgradeNodeViewModel vm)
        {
            _iconElement.style.backgroundImage = new StyleBackground(vm.Texture);
            _nameLabel.text = vm.NodeName;
            _descriptionLabel.text = vm.Description;

            _nodeViewModel = vm;

            //アルファベット以外を無くす。
            string word = Regex.Replace(vm.NodeName, "[^a-zA-Z]", "");
            _wordEntity = WordGenerator.GetWordEntity(word);

            _wordEntity.OnComplete += () => _isSelected = true;

            HandleWordUpdated(_wordEntity.Word, 0);
            _wordEntity.OnWordUpdated += HandleWordUpdated;
        }

        protected override Task Initialize_S(TemplateContainer container)
        {
            _wordLabel = container.Q<Label>(WORD_ELEMENT_NAME);
            _iconElement = container.Q<VisualElement>(ICON_ELEMENT_NAME);
            _nameLabel = container.Q<Label>(NAME_ELEMENT_NAME);
            _descriptionLabel = container.Q<Label>(EXPLANATION_ELEMENT_NAME);

            return Task.CompletedTask;
        }

        private const string WORD_ELEMENT_NAME = "word";
        private const string ICON_ELEMENT_NAME = "icon";
        private const string NAME_ELEMENT_NAME = "name";
        private const string EXPLANATION_ELEMENT_NAME = "explanation";

        private Label _wordLabel;
        private VisualElement _iconElement;
        private Label _nameLabel;
        private Label _descriptionLabel;

        private LevelUpgradeNodeViewModel _nodeViewModel;
        private WordEntityViewModel _wordEntity;
        private bool _isSelected;

        /// <summary>
        /// ワードの表示を更新します。
        /// </summary>
        /// <param name="word">表示するワード。</param>
        /// <param name="index">入力済みの文字数。</param>
        private void HandleWordUpdated(string word, int index)
        {
            _wordLabel.text = $"<b><color=green>{word[..index]}</color></b>{word[index..]}";
        }
    }
}
