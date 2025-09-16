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
            if (!gameObject.activeSelf) return;

            gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (!gameObject.activeSelf) return;

            gameObject.SetActive(false);
        }

        [SerializeField]
        private UnityEvent _onFire;
        [SerializeField]
        private CriAtomSource _source;
    }
}
