using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.Entity.Utility;
using UnityEngine;

namespace Cryptos.Runtime.UseCase.Ingame.System
{
    public class CriticalChanceStatusUpgrade : LevelUpgradeStatusEffect
    {
        public override void ApplyStatusEffect(TentativeCharacterData target)
        {
            var modifier = new StatModifier(_criticalChanceIncreaseAmount, StatModType.Multiplier, source: this);
            target.SetNewBuff(TentativeCharacterData.BuffType.CriticalChance, modifier);
        }

        [SerializeField, Min(0), Tooltip("クリティカル率増加量。(%)")]
        private float _criticalChanceIncreaseAmount = 0;
    }
}
