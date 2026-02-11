using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Cryptos.Runtime.UI.Outgame.Deck
{
    [UxmlElement]
    public partial class UIElementOutGameDeckEditorCard : VisualElementBase
    {
        public UIElementOutGameDeckEditorCard() : base("DeckEditorCard") { }

        protected override ValueTask Initialize_S(VisualElement root)
        {
            return default;
        }



    }
}
