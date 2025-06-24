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
        public CardInstance(CardData data, WordData[] wordDatas)
        {
            _data = data;
            _remainDifficulty = data.CardDifficulty;
            _wordDatas = wordDatas;

            SetNewWord();
        }

        public CardData CardData => _data;
        public WordData WordData => _wordData;

        [Tooltip("ワード入力が終了した時")]public event Action OnCompleteInput;
        [Tooltip("ワードの入力が更新された時")]public event Action<string, int> OnWordInputed;

        private CardData _data;
        private WordData _wordData;
        private WordData[] _wordDatas;

        private int _inputIndex;
        private int _remainDifficulty;

        /// <summary>
        ///     アルファベット入力を受けた時
        /// </summary>
        /// <param name="input"></param>
        public void OnInputChar(char input)
        {
            //次の文字が入力と同じか
            if (_wordData.Word[_inputIndex] == input)
            {
                _inputIndex++;
                if (CheckCompleteWord())
                {
                    OnCompleteInput?.Invoke();
                    return;
                }
                OnWordInputed?.Invoke(_wordData.Word, _inputIndex);
            }

            _inputIndex = 0; //入力が次の文字と同じじゃなければリセット
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

                if (_remainDifficulty <= 0 ) //進捗が終わったか
                {
                    OnCompleteInput?.Invoke();
                    return true;
                }
                else
                {
                    //進捗が終わっていなかったら新しいワードをセット
                    SetNewWord();
                }
            }

            return false;
        }

        /// <summary>
        ///     ランダムなワードをセットする
        /// </summary>
        private void SetNewWord()
        {
            int random = UnityEngine.Random.Range( 0, _wordDatas.Length );
            _wordData = _wordDatas[random];
            _inputIndex = 0;
        }
    }
}