using Cryptos.Runtime.Entity.Ingame.Character;
using UnityEngine;

namespace Cryptos.Runtime.UseCase.Ingame.System
{
    public class CriticalDamageStatusUpgrade : LevelUpgradeStatusEffect
    {
        public override void ApplyStatusEffect(TentativeCharacterData target)
        {
            target.SetNewBuff(TentativeCharacterData.BuffType.CriticalDamage,
                _criticalDamageIncreaseAmount);
        }

        [SerializeField, Min(0), Tooltip("クリティカル倍率増加量。(%)")]
        private float _criticalDamageIncreaseAmount = 0;
    }
}
