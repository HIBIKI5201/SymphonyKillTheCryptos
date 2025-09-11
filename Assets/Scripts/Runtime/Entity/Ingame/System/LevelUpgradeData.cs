using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.System
{
    [CreateAssetMenu(fileName = nameof(LevelUpgradeData), menuName = "Cryptos/" + nameof(LevelUpgradeData), order = 0)]
    public class LevelUpgradeData : ScriptableObject
    {
        public float[] LevelRequirePoints => _levelRequirePoints;
        public LevelUpgradeNode[] LevelCard => _levelCard;
        public int LevelUpgradeAmount => _levelUpgradeAmount;

        [SerializeField, Tooltip("各レベルアップに必要な経験値の配列。")]
        private float[] _levelRequirePoints = new float[] { 100f, 300f, 600f, 1000f };
        [SerializeField, Tooltip("レベルアップ時に提示されるカードの配列。")]
        private LevelUpgradeNode[] _levelCard;
        [SerializeField, Tooltip("レベルアップ時に提示されるカードの数。")]
        private int _levelUpgradeAmount = 3;
    }
}
