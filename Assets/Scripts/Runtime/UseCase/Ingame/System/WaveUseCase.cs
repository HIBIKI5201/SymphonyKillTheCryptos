using Cryptos.Runtime.Entity.Ingame.System;
using System;
using UnityEngine;

namespace Cryptos.Runtime.UseCase.Ingame.System
{
    /// <summary>
    ///     ウェーブの管理を行う。
    /// </summary>
    public class WaveUseCase
    {
        public WaveUseCase(WaveEntity[] waveEntities)
        {
            _waveEntities = waveEntities;
        }

        public event Action<WaveEntity> OnWaveChanged;

        /// <summary> 現在のウェーブ </summary>
        public WaveEntity CurrentWave => _waveEntities[_currentWaveIndex];

        /// <summary> 現在のウェーブのインデックス </summary>
        public int CurrentWaveIndex => _currentWaveIndex;

        /// <summary>
        ///     次のウェーブにする。
        /// </summary>
        /// <returns></returns>
        public WaveEntity NextWave()
        {
            if (_currentWaveIndex < _waveEntities.Length - 1)
            {
                _currentWaveIndex++;
            }
            else
            {
                Debug.LogWarning("No more waves available.");
            }

            WaveEntity newWave = _waveEntities[_currentWaveIndex];
            OnWaveChanged?.Invoke(newWave);

            return newWave;
        }

        private WaveEntity[] _waveEntities;

        private int _currentWaveIndex = 0;
    }
}