using SymphonyFrameWork.Utility;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cryptos.Runtime.UI.Ingame
{
    [UxmlElement]
    public partial class UIElementLevelUpgradeNode : SymphonyVisualElement
    {
        public UIElementLevelUpgradeNode() : base("UIToolKit/UXML/Ingame/LevelUpgradeNode",
            0)
        {
        }

        public void SetData(Texture2D icon, string name, string description)
        {
            _iconElement.style.backgroundImage = new StyleBackground(icon);
            _nameLabel.text = name;
            _descriptionLabel.text = description;
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
    }
}
