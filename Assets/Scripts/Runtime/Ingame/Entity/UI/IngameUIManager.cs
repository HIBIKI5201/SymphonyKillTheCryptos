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
        private UIDocument _document;

        private UIElementDeck _deck;

        private void Awake()
        {
            _document = GetComponent<UIDocument>();

            VisualElement root = _document.rootVisualElement;
            _deck = root.Q<UIElementDeck>();
        }

        [ContextMenu("AddCard")]
        private void AddCard() => _deck.AddCard();
    }
}