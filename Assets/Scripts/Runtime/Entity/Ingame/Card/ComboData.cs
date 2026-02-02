using Cryptos.Runtime.Entity.Utility;
using System;
using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Card
{
    public class ComboData
    {
        public ComboData(float comboDuration, ComboStackSpeed[] comboStackSpeeds)
        {
            _comboDuration = new(comboDuration);
            _comboStackSpeeds = comboStackSpeeds;
        }

        public float ComboDuration => _comboDuration.Value;

        public float GetComboStackSpeed(int combo)
        {
            if (combo < _comboStackSpeeds[0].RequireCombo) { return 1; }

            for (int i = 1; i <= _comboStackSpeeds.Length; i++)
            {
                ComboStackSpeed data = _comboStackSpeeds[^i];

                if (data.RequireCombo < combo)
                {
                    return data.Speed;
                }
            }

            return 1;
        }

        private DynamicFloatVariable _comboDuration;
        private ComboStackSpeed[] _comboStackSpeeds;
    }

    [Serializable]
    public struct ComboStackSpeed
    {
        public ComboStackSpeed(int requireCombo, float speed)
        {
            _requireCombo = requireCombo;
            _speed = speed;
        }

        public float RequireCombo => _requireCombo;
        public float Speed => _speed;

        [SerializeField]
        private int _requireCombo;
        [SerializeField]
        private float _speed;
    }
}
