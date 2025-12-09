using SymphonyFrameWork;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cryptos.Runtime.UI.Basis
{
    [RequireComponent(typeof(UIDocument))]
    public abstract class UIManagerBase : MonoBehaviour, IInitializeAsync
    {
        Task IInitializeAsync.InitializeTask { get; set; }

        async Task IInitializeAsync.InitializeAsync()
        {
            _document = GetComponent<UIDocument>();
            await InitializeDocumentAsync(_document, _document.rootVisualElement);
        }

        protected UIDocument _document;

        protected abstract Task InitializeDocumentAsync(UIDocument document, VisualElement root);
    }
}
