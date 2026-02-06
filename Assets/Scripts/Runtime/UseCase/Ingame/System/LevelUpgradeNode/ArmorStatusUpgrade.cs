using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.Entity.Utility;
using UnityEngine;

namespace Cryptos.Runtime.UseCase.Ingame.System
{
    public class ArmorStatusUpgrade : LevelUpgradeStatusEffect
    {
        public override void ApplyStatusEffect(TentativeCharacterData target)
        {
            StatModifier modifier = new(_armorIncreaseAmount, StatModType.Multiplier);
            target.SetNewBuff(TentativeCharacterData.BuffType.Armor, modifier);
        }

        [SerializeField, Min(0), Tooltip("装甲増加量。(%)")]
        private float _armorIncreaseAmount = 0;
    }
}
