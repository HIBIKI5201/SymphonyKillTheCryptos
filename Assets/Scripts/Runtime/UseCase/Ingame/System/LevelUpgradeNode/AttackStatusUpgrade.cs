using Cryptos.Runtime.Entity.Ingame.Character;
using UnityEngine;

namespace Cryptos.Runtime.UseCase.Ingame.System
{
    public class AttackStatusUpgrade : LevelUpgradeStatusEffect
    {
        public override void ApplyStatusEffect(TentativeCharacterData target)
        {
            target.SetNewBuff(TentativeCharacterData.BuffType.AttackPower,
                _attackIncreaseAmount);
        }

        [SerializeField, Min(0), Tooltip("攻撃力増加量。(%)")]
        private float _attackIncreaseAmount = 0;
    }
}
