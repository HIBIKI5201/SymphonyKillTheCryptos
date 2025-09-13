using Cryptos.Runtime.Entity.Ingame.Character;
using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.System
{
    public class ArmorStatusUpgrade : LevelUpgradeStatusEffect
    {
        public override void ApplyStatusEffect(TentativeCharacterData target)
        {
            target.SetNewBuff(TentativeCharacterData.BuffType.Armor,
                _armorIncreaseAmount);
        }

        [SerializeField, Min(0), Tooltip("装甲増加量。(%)")]
        private float _armorIncreaseAmount = 0;
    }
}
