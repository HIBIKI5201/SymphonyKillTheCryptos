using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Character
{
    public class PlayerCharacterEntity : CharacterEntity<SymphonyData>
    {
        public PlayerCharacterEntity(SymphonyData data) : base(data)
        {
            _symphonyData = data;
        }

        public override float GetAttackPower()
        {
            float power = _symphonyData.AttackPower;

            //クリティカル時に倍率を掛ける
            if (Random.Range(0, 1) < _symphonyData.CriticalChance)
            {
                power *= _symphonyData.CriticalDamage;
            }

            return power;
        }

        private readonly SymphonyData _symphonyData;
    }
}
