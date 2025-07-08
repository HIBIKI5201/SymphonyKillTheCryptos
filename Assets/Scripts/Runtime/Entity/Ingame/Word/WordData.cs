using SymphonyFrameWork.Attribute;
using System;
using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Word
{
    [Serializable]
    public class WordData
    {
        public WordData(string[] words, int difficulty)
        {
            _words = words;
            _difficulty = difficulty;
        }

        public string[] Words => _words;
        public int Difficulty => _difficulty;

        [SerializeField, ReadOnly, Tooltip("文字列")]
        private string[] _words;

        [SerializeField, ReadOnly, Tooltip("難易度"), Min(1)]
        private int _difficulty;
    }
}