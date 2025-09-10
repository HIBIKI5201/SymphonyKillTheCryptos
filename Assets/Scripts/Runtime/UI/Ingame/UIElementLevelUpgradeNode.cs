using Cryptos.Runtime.Presenter.Ingame.Word;
using SymphonyFrameWork.Utility;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;

namespace Cryptos.Runtime.UI.Ingame
{
    [UxmlElement]
    public partial class UIElementLevelUpgradeNode : SymphonyVisualElement
    {
        public UIElementLevelUpgradeNode() : base("UIToolKit/UXML/Ingame/LevelUpgradeNode", 0)
        {
        }

        public event Action OnComplete;

        public void OnInputChar(char c)
        {
            _wordEntity.InputChar(c);
        }

        public void SetData(Texture2D icon, string name, string description)
        {
            _iconElement.style.backgroundImage = new StyleBackground(icon);
            _nameLabel.text = name;
            _descriptionLabel.text = description;

            //アルファベット以外を無くす
            string word = Regex.Replace(name, "[^a-zA-Z]", "");
            _wordEntity = WordGenerator.GetWordEntity(word);

            HandleWordUpdated(_wordEntity.Word, 0);

            _wordEntity.OnComplete += () => OnComplete?.Invoke();
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

        private WordEntityViewModel _wordEntity;

        private void HandleWordUpdated(string word, int index)
        {
            _wordLabel.text = $"<b><color=green>{word[..index]}</color></b>{word[index..]}";
        }
    }
}
