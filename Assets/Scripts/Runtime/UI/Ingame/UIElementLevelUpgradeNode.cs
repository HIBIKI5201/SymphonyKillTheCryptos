using Cryptos.Runtime.Presenter.Ingame.Word;
using SymphonyFrameWork.Utility;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

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

            _wordEntity = WordGenerator.GetWordEntity(name);
            _wordEntity.OnComplete += OnComplete;
        }

        protected override Task Initialize_S(TemplateContainer container)
        {
            _iconElement = container.Q<VisualElement>("icon");
            _nameLabel = container.Q<Label>("name");
            _descriptionLabel = container.Q<Label>("description");

            return Task.CompletedTask;
        }

        private VisualElement _iconElement;
        private Label _nameLabel;
        private Label _descriptionLabel;

        private WordEntityViewModel _wordEntity;
    }
}
