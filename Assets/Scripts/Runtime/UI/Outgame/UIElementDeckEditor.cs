using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Cryptos.Runtime.UI.Outgame
{
    public class UIElementDeckEditor : VisualElementBase
    {
        public UIElementDeckEditor() : base("DeckEditor") { }

        protected override ValueTask Initialize_S(VisualElement root)
        {
            VisualElement cardHolder = root.Q<VisualElement>(CARD_HOLDER_NAME);

            return default;
        }

        private const string CARD_HOLDER_NAME = "card-holder";
    }
}
