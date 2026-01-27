using SymphonyFrameWork.Utility;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Cryptos.Runtime.UI
{
    [UxmlElement]
    public partial class UIElementComboCounter : SymphonyVisualElement
    {
        public UIElementComboCounter() : base("UIToolKit/UXML/InGame/ComboCounter", 
            initializeType: InitializeType.Absolute)
        {
        }

        public void SetCounter(int value)
        {
            _counter.text = value.ToString();
            CheckVisibility(value);
        }

        public void ResetCounter()
        {
            style.visibility = Visibility.Hidden;
        }

        public void SetGuage(float current, float max)
        {
            float rate = current / max;
            _guage.style.width = new Length(rate * 100, LengthUnit.Percent);
        }

        protected override Task Initialize_S(TemplateContainer container)
        {
            _counter = container.Q<Label>(COUNTER_NAME);
            _guage = container.Q(GUAGE_NAME);

            style.visibility = Visibility.Hidden;

            return Task.CompletedTask;
        }

        private const string COUNTER_NAME = "counter";
        private const string GUAGE_NAME = "guage";

        private Label _counter;
        private VisualElement _guage;

        [UxmlAttribute]
        private int _visibleComboCount = 3;

        private void CheckVisibility(int combo)
        {
            if (combo >= _visibleComboCount)
            {
                style.visibility = Visibility.Visible;
            }
            else
            {
                style.visibility = Visibility.Hidden;
            }
        }
    }
}
