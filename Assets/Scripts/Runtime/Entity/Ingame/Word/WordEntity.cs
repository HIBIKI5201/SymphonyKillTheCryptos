using System;
using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Word
{
    /// <summary>
    /// ワードタイピングのインスタンスデータを表すエンティティクラス。
    /// 個々のワードの入力進捗、難易度、および関連するイベントを管理します。
    /// </summary>
    [Serializable]
    public class WordEntity
    {
        /// <summary>
        /// WordEntityの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="initialWord">最初の単語</param>
        /// <param name="initialWordDifficulty">最初の単語の難易度</param>
        /// <param name="totalDifficulty">カード全体の総難易度</param>
        public WordEntity(string initialWord, int initialWordDifficulty, int totalDifficulty)
        {
            _difficulty = totalDifficulty;
            _remainDifficulty = totalDifficulty;
            SetNewWord(initialWord, initialWordDifficulty); // 最初の単語をセット
        }

        public event Action OnComplete; // カード全体のワード入力完了
        public event Action OnCurrentWordCompleted; // 現在の単語の入力完了
        public event Action<string, int> OnWordUpdated;
        public event Action<float> OnProgressUpdate;

        public string CurrentWord => _currentWord;

        /// <summary>
        /// UseCaseから次の単語をセットするための公開メソッド
        /// </summary>
        /// <param name="newWord">新しい単語</param>
        /// <param name="wordDifficulty">新しい単語の難易度</param>
        public void SetNewWord(string newWord, int wordDifficulty)
        {
            _matcher = new(newWord);
            _currentWord = newWord;
            _currentDifficulty = wordDifficulty;

            // UI更新イベントを発火
            OnWordUpdated?.Invoke(_currentWord, _matcher.Count);
            InvokeProgressEvent();
        }

        /// <summary>
        /// 文字入力を処理します。
        /// </summary>
        /// <param name="input">入力された文字</param>
        /// <returns>カード全体の入力が完了した場合true</returns>
        public bool OnInputChar(char input)
        {
            bool isCorrect = _matcher.ProccessInput(input);

            OnWordUpdated?.Invoke(_currentWord, _matcher.Count);

            if (_matcher.IsCompleted) // 現在の単語が完了
            {
                _remainDifficulty -= _currentDifficulty;
                OnCurrentWordCompleted?.Invoke(); // ★ 現在の単語が完了したことを通知

                if (_remainDifficulty <= 0) // 全てのワードが完了
                {
                    OnComplete?.Invoke();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     カウントをリセットする
        /// </summary>
        public void ResetIndex() => _matcher.ResetCount();

        private WordMatcher _matcher;
        private string _currentWord;
        private readonly float _difficulty;
        private float _remainDifficulty;
        private int _currentDifficulty;

        private void InvokeProgressEvent()
        {
            float progress = 1f - Mathf.Clamp01(_remainDifficulty / _difficulty);
            OnProgressUpdate?.Invoke(progress);
        }
    }
}
