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
        public float Value => _value;

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
            float result = 0f;

            IOrderedEnumerable<IGrouping<int, StatModifier>> groups = _modifiers
                .GroupBy(m => m.Priority)
                .OrderBy(g => g.Key);

            int i = 0;
            foreach (IGrouping<int, StatModifier> group in groups)
            {
                float value = 0f;

                // Priority0 はBaseValueを起点にする。
                if (i == 0)
                {
                    value = BaseValue;
                }

                // Additive を加算
                foreach (StatModifier add in group.Where(m => m.Type == StatModType.Additive))
                {
                    value += add.Value;
                }

                float sum = group.Where(m => m.Type == StatModType.Multiplier).Sum(m => m.Value);
                value *= 1 + sum / 100;

                result += value;
                i++;
            }

            return result;
        }
    }
}
