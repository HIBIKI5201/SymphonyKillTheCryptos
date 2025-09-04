using SymphonyFrameWork.Utility;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Cryptos.Runtime.UI.Ingame
{
    [UxmlElement]
    public partial class UIElementLevelUpgradeWindow : SymphonyVisualElement
    {
        public UIElementLevelUpgradeWindow() : base("UIToolKit/UXML/Ingame/LevelUpgradeWindow")
        {

        }

        protected override Task Initialize_S(TemplateContainer container)
        {


            return Task.CompletedTask;
        }
    }
}
