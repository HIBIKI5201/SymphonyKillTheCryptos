using Cryptos.Runtime.Entity.Ingame.Character;
using UnityEngine;

namespace Cryptos.Runtime.Entity
{
    public class HealthStatusUpgrade : ILevelUpgradeStatusEffect
    {
        public override void ApplyStatusEffect(TentativeCharacterData<CharacterData> target)
        {
            target.SetNewBuff(TentativeCharacterData<CharacterData>.BuffType.MaxHealth,
                _healthIncreaseAmount);
        }

        [SerializeField, Min(1)] private float _healthIncreaseAmount;
    }
}
