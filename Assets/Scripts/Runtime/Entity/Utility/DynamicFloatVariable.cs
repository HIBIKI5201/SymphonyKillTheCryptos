using System.Collections.Generic;
using System.Linq;
using UnityEngine;


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
            _baseValue = baseValue;
            _value = baseValue;
        }

        public float BaseValue => _baseValue;
        public float Value => _value;

        /// <summary>
        ///     乗算値を追加する
        /// </summary>
        /// <param name="multiplier"> バフの値。(%) </param>
        /// <param name="priority"> バフの優先度。 </param>
        public void AddMultiplier(float multiplier, int priority)
        {
            //リストが存在しない時は生成する。
            if (!_multipliers.TryGetValue(priority, out var list))
            {
                list = new List<float>() { multiplier };
                _multipliers.Add(priority, list);
            }
            else
            {
                list.Add(multiplier);
            }

            _value = Recalculate();
        }

        public void RemoveMultiplier(float multiplier, int priority)
        {
            if (_multipliers.TryGetValue(priority, out var list))
            {
                list.Remove(multiplier);
                _value = Recalculate();
            }
            else
            {
                Debug.LogWarning("Multiplier not registered");
            }
        }

        public void AddAdditive(float value)
        {
            _additives.Add(value);
            _value = Recalculate();
        }

        public void RemoveAdditive(float value)
        {
            _additives.Remove(value);
            _value = Recalculate();
        }

        private float _baseValue;
        private float _value;

        private SortedList<int, List<float>> _multipliers = new();
        private List<float> _additives = new();

        private float Recalculate()
        {
            float value = _baseValue;

            foreach (var add in _additives)
                value += add;

            foreach (var group in _multipliers)
                value *= 1 + group.Value.Sum() / 100;

            return value;
        }
    }
}
