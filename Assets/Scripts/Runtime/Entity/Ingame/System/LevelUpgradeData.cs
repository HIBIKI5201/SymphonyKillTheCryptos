using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.System
{
    [CreateAssetMenu(fileName = nameof(LevelUpgradeData), menuName = "Cryptos/" + nameof(LevelUpgradeData), order = 0)]
    public class LevelUpgradeData : ScriptableObject
    {
        public float[] LevelRequirePoints => _levelRequirePoints;
        public LevelUpgradeNode[] LevelCard => _levelCard;

        [SerializeField]
        private float[] _levelRequirePoints = new float[] { 100f, 300f, 600f, 1000f };
        [SerializeField]
        private LevelUpgradeNode[] _levelCard;
    }
}
