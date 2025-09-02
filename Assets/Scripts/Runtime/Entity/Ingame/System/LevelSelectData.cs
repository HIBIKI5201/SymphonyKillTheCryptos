using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.System
{
    public class LevelSelectData : ScriptableObject
    {
        public float[] LevelRequirePoints => _levelRequirePoints;
        public string[] LevelCard => _levelCard;

        [SerializeField]
        private float[] _levelRequirePoints = new float[] { 100f, 300f, 600f, 1000f };

        [SerializeField]
        private string[] _levelCard;
    }
}
