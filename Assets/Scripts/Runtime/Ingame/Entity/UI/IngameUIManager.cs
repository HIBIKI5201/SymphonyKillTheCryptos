using Cryptos.Runtime.Ingame.Entity;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cryptos.Runtime.Ingame.UI
{
    /// <summary>
    ///     インゲームのUIを管理する
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class IngameUIManager : MonoBehaviour
    {
        public UIElementDeck UIElementDeck => _deck;

        private UIDocument _document;

        private UIElementDeck _deck;

        [SerializeField]
        private CardData[] _cards;

        private void Awake()
        {
            _document = GetComponent<UIDocument>();

            VisualElement root = _document.rootVisualElement;
            _deck = root.Q<UIElementDeck>();
        }

        private void Start()
        {
            foreach (var card in _cards)
                _deck.AddCard(card);
        }
    }
}