using System;
using UnityEngine;

namespace Cryptos.Runtime.Ingame.Entity
{
    /// <summary>
    ///     カードのインスタンスデータ
    /// </summary>
    public class CardInstance
    {
        /// <summary>
        ///     インスタンスの初期化
        /// </summary>
        /// <param name="data">カードのデータ</param>
        /// <param name="wordDatas">出てくるワードの候補</param>
        public CardInstance(CardData data, WordData[] wordDatas, WordManager wordManager)
        {
            _data = data;
            _wordManager = wordManager;
            _candidateWordDatas = wordDatas;
            _remainDifficulty = data.CardDifficulty;

            NextWord();
        }

        [Tooltip("ワード入力が終了した時")] public event Action<CardInstance> OnComplete;
        [Tooltip("ワードの入力が更新された時")] public event Action<string, int> OnWordUpdated;
        [Tooltip("ワードコンプリート進捗率")] public event Action<float> OnProgressUpdate;

        public CardData CardData => _data;
        public string CurrentWord => _currentData.word;

        /// <summary>
        ///     アルファベット入力を受けた時
        /// </summary>
        /// <param name="input"></param>
        public void OnInputChar(char input)
        {
            //次の文字が入力と同じか
            if (IsMatch(input))
            {
                SuccessInput(); //同じなら成功入力
            }
            else
            {
                _inputIndex = 0; //入力が次の文字と同じじゃなければリセット

                if (IsMatch(input)) //リセット後に最初の文字が同じなら成功
                {
                    SuccessInput();
                }
            }

            OnWordUpdated?.Invoke(_currentData.word, _inputIndex);
        }

        private CardData _data;
        private WordManager _wordManager;

        [Tooltip("候補となるワード一覧")] private WordData[] _candidateWordDatas;
        private (int difficulty, string word) _currentData;


        private int _inputIndex;
        private int _remainDifficulty;

        /// <summary>
        ///     ワードの入力が成功した時の処理
        /// </summary>
        private void SuccessInput()
        {
            _inputIndex++;
            if (CheckCompleteWord())
            {
                _wordManager.RemoveWord(_currentData.word);
                OnComplete?.Invoke(this);
            }
        }

        /// <summary>
        ///     ワードの入力が全て終了したか判定
        /// </summary>
        /// <returns></returns>
        private bool CheckCompleteWord()
        {
            if (_currentData.word.Length <= _inputIndex) //ワードが全部書き終わったか
            {
                _remainDifficulty -= _currentData.difficulty; //難易度に応じて進捗を更新

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
            _wordManager.RemoveWord(_currentData.word);

            //ランダムなワードを取得する
            var data = _wordManager.GetAvailableWord(_candidateWordDatas);
            if (data.difficulty < 0) return;

            //新しいワードを登録しなおす
            _wordManager.AddWord(data.word);
            _currentData = data;
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

        private bool IsMatch(char input) => _currentData.word.ToUpper()[_inputIndex] == input;
    }
}