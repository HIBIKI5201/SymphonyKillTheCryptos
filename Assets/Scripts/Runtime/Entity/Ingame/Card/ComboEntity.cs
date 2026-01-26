using Cryptos.Runtime.Entity.Ingame.Character;
using System;
using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Card
{
    public class ComboEntity
    {
        public ComboEntity(SymphonyData playerData)
        {
            _playerData = playerData;
        }

        /// <summary> 第一引数は現在値、第二引数は最大値。0になるとコンボリセット。 </summary>
        public event Action<float, float> OnChangedTimer;

        /// <summary> コンボカウントが変更される。 </summary>
        public event Action<int> OnChangedCounter;
        /// <summary> コンボカウントがリセットされる。 </summary>
        public event Action OnComboReset;

        public void Tick(float delta)
        {
            if (0 <= _timer)
            {
                _timer = Mathf.Max(_timer - delta, 0);
                OnChangedTimer?.Invoke(_timer, _playerData.ComboDuration);
            }
        }

        public void AddCount()
        {
            _count++;
            OnChangedCounter?.Invoke(_count);
        }

        public void Reset()
        {
            _count = 0;
            OnComboReset?.Invoke();
        }

        private SymphonyData _playerData;

        private int _count = 0;
        private float _timer = 0;
    }
}
