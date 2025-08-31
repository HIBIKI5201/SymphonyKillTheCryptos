using System;

namespace Cryptos.Runtime.Entity.Ingame.System
{
    public class LevelEntity
    {
        public event Action<int> OnLevelChanged;
        public event Action<float> OnLevelProgressChanged;

        public int CurrentLevel => _currentLevel;
        public float LevelProgress => _levelProgress;

        public void AddLevelProgress(float value)
        {
            ChangeLevelProgress(value + _levelProgress);
        }

        private int _currentLevel;
        private float _levelProgress;

        /// <summary>
        /// レベル進行度を変更する
        /// 1を超えた場合、レベルアップする
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private void ChangeLevelProgress(float value)
        {
            //1以下になるまでレベルアップ処理を行う
            while (1 < value)
            {
                value -= 1;
                ChangeLevel(_currentLevel + 1);
            }

            _levelProgress = value;
            OnLevelProgressChanged?.Invoke(value);
        }

        /// <summary>
        /// レベルを変更する
        /// </summary>
        /// <param name="value"></param>
        private void ChangeLevel(int value)
        {
            _currentLevel = value;
            OnLevelChanged?.Invoke(value);
        }
    }
}
