using UnityEngine;

namespace Cryptos.Runtime
{
    public class TimeScaleChanger : MonoBehaviour
    {
        [SerializeField]
        private float _timeSlale;

        [ContextMenu(nameof(SetTimeScale))]
        private void SetTimeScale()
        {
            Time.timeScale = _timeSlale;
        }
    }
}
