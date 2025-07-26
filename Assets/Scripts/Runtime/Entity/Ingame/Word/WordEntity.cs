using Cryptos.Runtime.Entity.Ingame.Word;
using System;
using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Word
{
    /// <summary>
    ///     カードのインスタンスデータ
    /// </summary>
    [Serializable]
    public class WordEntity
    {
        /// <summary>
        ///     インスタンスの初期化
        /// </summary>
        /// <param name="data">カードのデータ</param>
        /// <param name="wordDatas">出てくるワードの候補</param>
        public WordEntity(CardData data, WordData[] wordDatas, WordManager wordManager)
        {
            _data = data;
            _wordManager = wordManager;
            _wordCandidates = wordDatas;
            _remainDifficulty = data.CardDifficulty;

            NextWord();
        }

        [Tooltip("ワード入力が終了した時")] public event Action OnComplete;
        [Tooltip("ワードの入力が更新された時")] public event Action<string, int> OnWordUpdated;
        [Tooltip("ワードコンプリート進捗率")] public event Action<float> OnProgressUpdate;

        public CardData CardData => _data;
        public string CurrentWord => _currentWord;

        /// <summary>
        ///     アルファベット入力を受けた時
        /// </summary>
        /// <param name="input"></param>
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
            if (CheckCompleteWord()) 
            {
                _wordManager.RemoveWord(_currentWord);
                OnComplete?.Invoke();
                return true;
            }

            OnWordUpdated?.Invoke(_currentWord, _inputIndex);
            return false;
        }

        private CardData _data;
        private WordManager _wordManager;

        [Tooltip("候補となるワード一覧")] private WordData[] _wordCandidates;

        private int _currentDifficulty;
        private string _currentWord;

        private int _inputIndex;
        private int _remainDifficulty;

        /// <summary>
        ///     ワードの入力が全て終了したか判定
        /// </summary>
        /// <returns></returns>
        private bool CheckCompleteWord()
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
        ///     次のワードに変更する
        /// </summary>
        private void NextWord()
        {
            SetNewWord();
            InvokeProgressEvent();
        }

        /// <summary>
        ///     ランダムなワードをセットする
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
        ///     現在の進捗率を引数にイベントを発火する
        /// </summary>
        private void InvokeProgressEvent()
        {
            float progress = 1f - Mathf.Clamp01((float)_remainDifficulty / (float)_data.CardDifficulty);
            OnProgressUpdate?.Invoke(progress);
        }

        private bool IsMatch(char input) => _currentWord.ToUpper()[_inputIndex] == input;
    }
}