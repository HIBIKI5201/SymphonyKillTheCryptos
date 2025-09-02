using System;
using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.System
{
    /// <summary>
    ///     レベル管理を行うエンティティ
    /// </summary>
    public class LevelEntity
    {
        public LevelEntity(float[] requirePoints)
        {
            _currentLevel = 1;
            _levelProgress = 0;

            _requirePoints = requirePoints;
        }

        public event Action<int> OnLevelChanged;
        public event Action<float> OnLevelProgressChanged;

        public int CurrentLevel => _currentLevel;
        public float LevelProgress => _levelProgress;

        /// <summary>
        ///     レベル進行度を増加させます。
        /// </summary>
        /// <param name="value"></param>
        public void AddLevelProgress(float value)
        {
            if (_requirePoints.Length <= _currentLevel)
            {
                Debug.Log("player is max Level");
                return;
            }

            //必要経験値に対する割合を計算
            float proportion = value / _requirePoints[_currentLevel - 1];
            //現在の進行度に加算
            float progress = proportion + _levelProgress;

            ChangeLevelProgress(progress);
        }

        private int _currentLevel;
        private float _levelProgress;

        private float[] _requirePoints;

        /// <summary>
        ///     レベル進行度を変更します。
        ///     1を超えた場合、レベルアップします。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private void ChangeLevelProgress(float value)
        {
            //進行度が1以下になるまでレベルアップ処理を行う
            while (1 < value)
            {
                value -= 1;
                ChangeLevel(_currentLevel + 1);
            }

            _levelProgress = value;
            OnLevelProgressChanged?.Invoke(value);
        }

        /// <summary>
        ///     レベルを変更します。
        /// </summary>
        /// <param name="value"></param>
        private void ChangeLevel(int value)
        {
            _currentLevel = value;
            OnLevelChanged?.Invoke(value);
        }
    }
}
