using System;
using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.System
{
    /// <summary>
    ///     レベル管理を行うエンティティ。
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
            // レベルアップを繰り返す可能性があるためループで処理。
            // 最大レベルに達するか、取得経験値がなくなるまで続ける。
            while (_currentLevel < _requirePoints.Length && 0 < value)
            {
                float required = _requirePoints[_currentLevel - 1]; // 次のレベルに必要な経験値。
                float remaining = (1f - _levelProgress) * required; // 現在の進行度からレベルアップまでに必要な残り経験値。

                if (value >= remaining)
                {
                    // 取得経験値が残り経験値を超えている場合レベルアップ。

                    value -= remaining; // 残り経験値を算出。

                    ChangeLevel(_currentLevel + 1);
                    ChangeLevelProgress(0f);
                }
                else
                {
                    // レベルアップに届かない場合は進捗だけ増加。

                    float progress = (value / required) + _levelProgress;
                    ChangeLevelProgress(progress);

                    value = 0; // 余りを無くしてループを抜ける。
                }
            }
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
