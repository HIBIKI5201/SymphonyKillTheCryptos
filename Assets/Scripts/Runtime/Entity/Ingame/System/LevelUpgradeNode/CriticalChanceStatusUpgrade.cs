using Cryptos.Runtime.Entity.Ingame.Character;
using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.System
{
    public class CriticalChanceStatusUpgrade : LevelUpgradeStatusEffect
    {
        public override void ApplyStatusEffect(TentativeCharacterData target)
        {
            target.SetNewBuff(TentativeCharacterData.BuffType.CriticalChance,
                _criticalChanceIncreaseAmount);
        }

        [SerializeField, Min(0), Tooltip("クリティカル率増加量。(%)")]
        private float _criticalChanceIncreaseAmount = 0;
    }
}
