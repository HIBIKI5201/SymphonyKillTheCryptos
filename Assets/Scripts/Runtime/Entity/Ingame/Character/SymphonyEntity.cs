using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Character
{
    public class SymphonyEntity : CharacterEntity
    {
        public SymphonyEntity(ICharacterData data) : base(data)
        {
            if (data is TentativeCharacterData tentativeCharacterData)
            {
                tentativeCharacterData.OnStatusChanged += HandleStatusChanged;
            }
        }

        private void HandleStatusChanged(TentativeCharacterData.BuffType type, float oldValue, float newValue)
        {
            if (type == TentativeCharacterData.BuffType.MaxHealth)
            {
                //増加した量を回復する
                float addedHealth = newValue - oldValue;
                _healthEntity.AddHealthHeal(addedHealth);
            }
        }
    }
}
