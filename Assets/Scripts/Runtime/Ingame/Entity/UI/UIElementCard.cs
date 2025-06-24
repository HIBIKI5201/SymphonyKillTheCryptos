using Cryptos.Runtime.Ingame.Entity;
using SymphonyFrameWork.Utility;
using System;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Cryptos.Runtime.Ingame.UI
{
    [UxmlElement]
    public partial class UIElementCard : SymphonyVisualElement
    {
        public UIElementCard() : base("UIToolKit/UXML/Ingame/Card", InitializeType.PickModeIgnore) { }

        public event Action OnDispose;

        private const int SIDE_MARGIN = 10;

        private CardData _cardData;

        /// <summary>
        ///     データをセットする
        /// </summary>
        /// <param name="data"></param>
        public void SetData(CardData data)
        {
            _cardData = data;
        }

        protected override Task Initialize_S(TemplateContainer container)
        {
            style.marginRight = SIDE_MARGIN;
            style.marginLeft = SIDE_MARGIN;

            return Task.CompletedTask;
        }


    }
}
