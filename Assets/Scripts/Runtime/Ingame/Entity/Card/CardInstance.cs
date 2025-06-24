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

        public CardData CardData => _data;
        public WordData WordData => _wordData;

        [Tooltip("ワード入力が終了した時")] public event Action OnCompleteInput;
        [Tooltip("ワードの入力が更新された時")] public event Action<string, int> OnWordInputed;
        [Tooltip("ワードコンプリート進捗率")] public event Action<float> OnProgressUpdate;

        private CardData _data;
        private WordManager _wordManager;
        private WordData _wordData;
        [Tooltip("候補となるワード一覧")] private WordData[] _candidateWordDatas;

        private int _inputIndex;
        private int _remainDifficulty;

        /// <summary>
        ///     アルファベット入力を受けた時
        /// </summary>
        /// <param name="input"></param>
        public void OnInputChar(char input)
        {
            //次の文字が入力と同じか
            if (_wordData.Word.ToUpper()[_inputIndex] == input)
            {
                _inputIndex++;
                if (CheckCompleteWord())
                {
                    OnCompleteInput?.Invoke();
                    return;
                }
            }
            else
            {
                _inputIndex = 0; //入力が次の文字と同じじゃなければリセット
            }

            OnWordInputed?.Invoke(_wordData.Word, _inputIndex);
        }

        /// <summary>
        ///     ワードの入力が終了したか判定
        /// </summary>
        /// <returns></returns>
        private bool CheckCompleteWord()
        {
            if (_wordData.Word.Length <= _inputIndex) //ワードが全部書き終わったか
            {
                _remainDifficulty -= _wordData.Difficulty; //難易度に応じて進捗を更新

                if (_remainDifficulty <= 0) //進捗が終わったか
                {
                    OnCompleteInput?.Invoke();
                    _wordManager.RemoveWord(_wordData);
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
            _wordManager.RemoveWord(_wordData);

            //ランダムなワードを取得する
            WordData word = _wordManager.GetAvailableWord(_candidateWordDatas);
            if (word == null) return;

            //新しいワードを登録しなおす
            _wordManager.AddWord(word);
            
            _wordData = word;
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
    }
}