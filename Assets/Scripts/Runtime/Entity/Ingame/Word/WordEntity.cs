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
        /// <param name="wordDatas">ワードの候補データ配列</param>
        /// <param name="wordManager">ワードの管理を行うマネージャー</param>
        /// <param name="difficulty">このワードエンティティの初期難易度</param>
        public WordEntity(WordData[] wordDatas, WordManager wordManager, int difficulty)
        {
            _wordManager = wordManager;
            _wordCandidates = wordDatas;
            _difficulty = difficulty;
            _remainDifficulty = difficulty;

            NextWord();
        }

        [Tooltip("ワード入力が全て完了した時に発生するイベント")] public event Action OnComplete;
        [Tooltip("現在のワードの入力状況が更新された時に発生するイベント。引数は現在のワードと入力済みの文字数。")] public event Action<string, int> OnWordUpdated;
        [Tooltip("ワードコンプリートの進捗率が更新された時に発生するイベント。引数は0.0fから1.0fの進捗率。")] public event Action<float> OnProgressUpdate;

        /// <summary>
        /// 現在入力対象となっているワードを取得します。
        /// </summary>
        public string CurrentWord => _currentWord;

        /// <summary>
        /// アルファベット入力を受け取った際の処理。
        /// 入力文字が現在のワードと一致するかを判定し、進捗を更新します。
        /// </summary>
        /// <param name="input">入力された文字</param>
        /// <returns>このワードエンティティの全てのワード入力が完了した場合はtrue、それ以外はfalse。</returns>
        public bool OnInputChar(char input)
        {
            // 入力が次の期待文字と一致しない場合は進捗をリセットする
            if (!IsMatch(input))
            {
                _inputIndex = 0;

                // リセット後に先頭文字と一致しない場合は処理を終了
                if (!IsMatch(input))
                {
                    OnWordUpdated?.Invoke(_currentWord, _inputIndex);
                    return false;
                }
            }

            // 一致しているので進捗を進める
            _inputIndex++;
            //終了しているか判定
            if (UpdateWord())
            {
                _wordManager.RemoveWord(_currentWord);
                OnComplete?.Invoke();
                return true;
            }

            OnWordUpdated?.Invoke(_currentWord, _inputIndex);
            return false;
        }

        private WordManager _wordManager;

        [Tooltip("候補となるワード一覧")] private WordData[] _wordCandidates;

        private int _currentDifficulty;
        private string _currentWord;

        private int _inputIndex;
        private readonly float _difficulty;
        private float _remainDifficulty;

        /// <summary>
        /// 現在のワードの入力が全て終了したか、またはワードエンティティ全体の進捗が完了したかを判定し、更新します。
        /// </summary>
        /// <returns>ワードエンティティ全体の進捗が完了した場合はtrue、それ以外はfalse。</returns>
        private bool UpdateWord()
        {
            if (_currentWord.Length <= _inputIndex) //ワードが全部書き終わったか
            {
                _remainDifficulty -= _currentDifficulty; //難易度に応じて進捗を更新

                if (_remainDifficulty <= 0) //進捗が終わったか
                {
                    return true;
                }
                else
                {
                    //進捗が終わっていなかったら新しいワードをセット
                    NextWord();
                }
            }

            return false;
        }

        /// <summary>
        /// 次のワードに変更し、関連するイベントを発火します。
        /// </summary>
        private void NextWord()
        {
            SetNewWord();
            InvokeProgressEvent();
        }

        /// <summary>
        /// ランダムなワードをセットし、WordManagerに登録します。
        /// </summary>
        private void SetNewWord()
        {
            _wordManager.RemoveWord(_currentWord);

            //ランダムなワードを取得する
            var data = _wordManager.GetAvailableWord(_wordCandidates);
            if (data.difficulty < 0) return;

            //新しいワードを登録しなおす
            _wordManager.AddWord(data.word);
            (_currentDifficulty, _currentWord) = data;
            _inputIndex = 0;
        }

        /// <summary>
        /// 現在のワードコンプリート進捗率を計算し、OnProgressUpdateイベントを発火します。
        /// </summary>
        private void InvokeProgressEvent()
        {
            float progress = 1f - Mathf.Clamp01(_remainDifficulty / _difficulty);
            OnProgressUpdate?.Invoke(progress);
        }

        /// <summary>
        /// 入力された文字が現在のワードの次の期待文字と一致するかを判定します。
        /// </summary>
        /// <param name="input">入力された文字</param>
        /// <returns>一致する場合はtrue、それ以外はfalse。</returns>
        private bool IsMatch(char input) => _currentWord.ToUpper()[_inputIndex] == input;
    }
}
