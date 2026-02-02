using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.Entity.Utility;
using UnityEngine;

namespace Cryptos.Runtime.UseCase.Ingame.System
{
    public class CriticalDamageStatusUpgrade : LevelUpgradeStatusEffect
    {
        public override void ApplyStatusEffect(TentativeCharacterData target)
        {
            var modifier = new StatModifier(_criticalDamageIncreaseAmount, StatModType.Multiplier, source: this);
            target.SetNewBuff(TentativeCharacterData.BuffType.CriticalDamage, modifier);
        }

        [SerializeField, Min(0), Tooltip("クリティカル倍率増加量。(%)")]
        private float _criticalDamageIncreaseAmount = 0;
    }
}
