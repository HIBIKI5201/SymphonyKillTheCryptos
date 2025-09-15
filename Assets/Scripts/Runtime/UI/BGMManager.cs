using CriWare;
using UnityEngine;

namespace Cryptos.Runtime.UI.System.Audio
{
    [RequireComponent(typeof(CriAtomSource))]
    public class BGMManager : MonoBehaviour
    {
        public void PlayBGM(string cue)
        {
            if (_currentCueName == cue) return; // 既に再生中ならしない。

            _source.cueName = cue;
            _playback = _source.Play();
            _currentCueName = cue;
        }

        private CriAtomSource _source;

        private CriAtomExPlayback _playback;
        private string _currentCueName;

        private void Awake()
        {
            _source = GetComponent<CriAtomSource>();
        }
    }
}
