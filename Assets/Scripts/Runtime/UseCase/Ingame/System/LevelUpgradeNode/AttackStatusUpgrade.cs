using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.Entity.Utility;
using UnityEngine;

namespace Cryptos.Runtime.UseCase.Ingame.System
{
    public class AttackStatusUpgrade : LevelUpgradeStatusEffect
    {
        public override void ApplyStatusEffect(TentativeCharacterData target)
        {
            StatModifier modifier = new(_attackIncreaseAmount, StatModType.Multiplier);
            target.SetNewBuff(TentativeCharacterData.BuffType.AttackPower, modifier);
        }

        [SerializeField, Min(0), Tooltip("攻撃力増加量。(%)")]
        private float _attackIncreaseAmount = 0;
    }
}
