using SymphonyFrameWork.Attribute;
using System;
using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Word
{
    /// <summary>
    /// 単語のデータ構造を表すクラス。
    /// 複数の単語文字列とそれに対応する難易度を保持します。
    /// </summary>
    [Serializable]
    public class WordData
    {
        /// <summary>
        /// WordDataの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="words">単語文字列の配列</param>
        /// <param name="difficulty">この単語データの難易度</param>
        public WordData(string[] words, int difficulty)
        {
            _words = words;
            _difficulty = difficulty;
        }

        /// <summary>
        /// 単語文字列の配列を取得します。
        /// </summary>
        public string[] Words => _words;

        /// <summary>
        /// この単語データの難易度を取得します。
        /// </summary>
        public int Difficulty => _difficulty;

        [SerializeField, ReadOnly, Tooltip("文字列")]
        private string[] _words;

        [SerializeField, ReadOnly, Tooltip("難易度"), Min(1)]
        private int _difficulty;
    }
}
