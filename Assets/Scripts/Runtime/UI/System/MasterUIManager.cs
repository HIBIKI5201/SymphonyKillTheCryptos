using Cryptos.Runtime.Presenter.System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using SymphonyFrameWork.System;

namespace Cryptos.Runtime.UI.Basis 
{
    /// <summary>
    ///     共通UIのマネージャークラス。
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class MasterUIManager : MonoBehaviour, IMasterUIManager
    {
        public async Task FadeOut(float duration, CancellationToken token = default)
        {
            float elapsed = 0;
            Color baseColor = _fadeGround.style.backgroundColor.value;

            while (!token.IsCancellationRequested && elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                baseColor.a = t;
                _fadeGround.style.backgroundColor = baseColor;
                await Awaitable.NextFrameAsync(token);
            }

            baseColor.a = 1;
            _fadeGround.style.backgroundColor = baseColor;
        }

        public async Task FadeIn(float duration, CancellationToken token = default)
        {
            float elapsed = duration;
            Color baseColor = _fadeGround.style.backgroundColor.value;

            while (!token.IsCancellationRequested && 0 < elapsed)
            {
                elapsed -= Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                baseColor.a = t;
                _fadeGround.style.backgroundColor = baseColor;
                await Awaitable.NextFrameAsync(token);
            }

            baseColor.a = 0;
            _fadeGround.style.backgroundColor = baseColor;
        }


        private const string FADE_GROUND = "fade-ground";
        private UIDocument _document;
        private VisualElement _fadeGround;
        
        private void Awake()
        {
            ServiceLocator.RegisterInstance<IMasterUIManager>(this, ServiceLocator.LocateType.Locator);
            _document = GetComponent<UIDocument>();
        }

        private void Start()
        {
            if (_document == null) { return; }

            VisualElement root = _document.rootVisualElement;
            _fadeGround = root.Q<VisualElement>(FADE_GROUND);
        }
    }
}
