using CriWare;
using UnityEngine;
using UnityEngine.Events;

namespace Cryptos.Runtime.UI.Ingame.Character.Player
{
    public class FirearmAnimeManager : MonoBehaviour
    {
        public void Fire()
        {
            _source?.Play();

            _onFire.Invoke();
        }

        public void Show()
        {
            if (_visible) return;

            _visible = true;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (!_visible) return;

            _visible = false;
            gameObject.SetActive(false);
        }

        [SerializeField]
        private UnityEvent _onFire;
        [SerializeField]
        private CriAtomSource _source;

        private bool _visible;
    }
}
