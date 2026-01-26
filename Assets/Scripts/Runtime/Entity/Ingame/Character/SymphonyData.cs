

using Cryptos.Runtime.Entity.Utility;

namespace Cryptos.Runtime.Entity.Ingame.Character
{
    public class SymphonyData : ICharacterData
    {
        public SymphonyData(TentativeCharacterData data)
        {
            _characterData = data;
        }

        public string Name => _characterData.Name;
        public float AttackPower => _characterData.AttackPower;

        public float CriticalChance => _characterData.CriticalChance;

        public float CriticalDamage => _characterData.CriticalDamage;

        public float MaxHealth => _characterData.MaxHealth;

        public float Armor => _characterData.Armor;
        public float ComboDuration => _comboDuration.Value;

        private readonly TentativeCharacterData _characterData;
        private readonly DynamicFloatVariable _comboDuration;
    }
}
