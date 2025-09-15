using CriWare;
using UnityEngine;
using UnityEngine.Events;

namespace Cryptos.Runtime.UI
{
    public class FirearmAnimeManager : MonoBehaviour
    {
        public void Fire()
        {
            _source?.Play();

            _onFire.Invoke();
        }

        [SerializeField]
        private UnityEvent _onFire;
        [SerializeField]
        private CriAtomSource _source;
    }
}
