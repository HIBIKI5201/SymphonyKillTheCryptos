using SymphonyFrameWork.Utility;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Cryptos.Runtime.UI.Ingame
{
    [UxmlElement]
    public partial class UIElementLevelUpgradeNode : SymphonyVisualElement
    {
        public UIElementLevelUpgradeNode() : base("UIToolKit/UXML/Ingame/LevelUpgradeNode")
        {
        }

        protected override Task Initialize_S(TemplateContainer container)
        {
            return Task.CompletedTask;
        }
    }
}
