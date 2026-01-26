using Cryptos.Runtime.Entity.Ingame.Card;

namespace Cryptos.Runtime.Entity.Ingame.Character
{
    public class SymphonyData : ICharacterData
    {
        public SymphonyData(TentativeCharacterData data, ComboData comboData)
        {
            _characterData = data;
            _comboData = comboData;
        }

        public string Name => _characterData.Name;
        public float AttackPower => _characterData.AttackPower;

        public float CriticalChance => _characterData.CriticalChance;

        public float CriticalDamage => _characterData.CriticalDamage;

        public float MaxHealth => _characterData.MaxHealth;

        public float Armor => _characterData.Armor;
        public float ComboDuration => _comboData.ComboDuration;
        public float GetComboStackSpeed(int combo) => _comboData.GetComboStackSpeed(combo);

        private readonly TentativeCharacterData _characterData;
        private readonly ComboData _comboData;
    }
}
