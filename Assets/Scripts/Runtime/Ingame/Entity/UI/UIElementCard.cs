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

        private CardInstance _cardInstance;

        private Label _wordLabel;

        /// <summary>
        ///     データをセットする
        /// </summary>
        /// <param name="data"></param>
        public void SetData(CardInstance instance)
        {
            _cardInstance = instance;
            instance.OnWordInputed += OnWordUpdate;
            OnWordUpdate(instance.WordData.Word, 0);
        }

        protected override Task Initialize_S(TemplateContainer container)
        {
            style.marginRight = SIDE_MARGIN;
            style.marginLeft = SIDE_MARGIN;

            _wordLabel = container.Q<Label>("word");

            return Task.CompletedTask;
        }

        private void OnWordUpdate(string word, int index)
        {
            string newText = "<b>" + word.Substring(0, index) + "</b>" + word.Substring(index, word.Length - index);
            _wordLabel.text = newText;
        }
    }
}
