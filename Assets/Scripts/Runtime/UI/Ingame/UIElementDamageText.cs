using Cryptos.Runtime.Presenter.Ingame.Character;
using SymphonyFrameWork.Utility;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cryptos.Runtime.UI
{
    [UxmlElement]
    public partial class UIElementDamageText : SymphonyVisualElement
    {
        public UIElementDamageText() : base("UIToolKit/UXML/Ingame/DamageText")
        {

        }

        public void SetData(CombatContextViewModel context, Vector3 position)
        {
            _textLabel.text = context.Damage.ToString();

            if (0 < context.CriticalCount)
            {
                Color color = context.CriticalCount switch
                {
                    1 => Color.yellow,
                    2 => Color.orange,
                    _ => Color.red,
                };

                _textLabel.style.color = color;
            }

            _backGround.RegisterCallback<GeometryChangedEvent>(PositionChanged);

            void PositionChanged(GeometryChangedEvent e)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(position);
                Vector2 center = new(screenPos.x, screenPos.y);

                //背景の中心座標。
                Vector2 size = new Vector2(
                    _backGround.resolvedStyle.width,
                    _backGround.resolvedStyle.height)
                    / 2;

                Vector2 selfPivot = center - size;

                //UITK座標系では値が高いほど下に移動する
                selfPivot = new(selfPivot.x, Screen.height - selfPivot.y);

                SetPosition(selfPivot);

                _backGround.UnregisterCallback<GeometryChangedEvent>(PositionChanged); //イベント解放。
            }
        }

        protected override Task Initialize_S(TemplateContainer container)
        {
            _backGround = container.Q<VisualElement>(BACKGROUND_NAME);
            _textLabel = container.Q<Label>(TEXT_NAME);

            return Task.CompletedTask;
        }

        private const string BACKGROUND_NAME = "back";
        private const string TEXT_NAME = "text";

        private VisualElement _backGround;
        private Label _textLabel;

        private void SetPosition(Vector2 position)
        {
            style.left = position.x;
            style.top = position.y;
        }
    }
}
