using System.Collections.Generic;
using System.Linq;

namespace Cryptos.Runtime.Entity.Utility
{
    /// <summary>
    ///     バフによる状態変化が可能なfloat変数
    /// </summary>
    public class DynamicFloatVariable
    {        
        /// <summary>
        ///     初期の量を決定する
        /// </summary>
        /// <param name="baseValue"></param>
        public DynamicFloatVariable(float baseValue)
        {
            _bsseValue = baseValue;
            _value = baseValue;
        }

        public float BaseValue => _bsseValue;
        public float Value => _bsseValue;

        public void AddModifier(StatModifier mod)
        {
            _modifiers.Add(mod);
            _value = Recalculate();
        }

        public void RemoveModifier(StatModifier mod)
        {
            _modifiers.Remove(mod);
            _value = Recalculate();
        }

        private readonly float _bsseValue;
        private float _value;
        private readonly List<StatModifier> _modifiers = new();

        private float Recalculate()
        {
            float finalValue = BaseValue;
            
            // 加算Modifierを先に適用。
            foreach (var mod in _modifiers.Where(m => m.Type == StatModType.Additive))
            {
                finalValue += mod.Value;
            }

            // 乗算Modifierを優先度順に適用。
            var multiplierGroups = _modifiers.Where(m => m.Type == StatModType.Multiplier)
                .GroupBy(m => m.Priority)
                .OrderBy(g => g.Key);

            foreach (var group in multiplierGroups)
            {
                float totalMultiplier = 1f + group.Sum(mod => mod.Value) / 100f;
                finalValue *= totalMultiplier;
            }

            return finalValue;
        }
    }
}
