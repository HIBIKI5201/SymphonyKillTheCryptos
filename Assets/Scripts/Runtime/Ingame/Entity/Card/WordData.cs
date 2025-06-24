using System;
using UnityEngine;

namespace Cryptos.Runtime.Ingame.Entity
{
    [Serializable]
    public class WordData
    {
        public string Word => _word;
        public int Difficulty => _difficulty;

        [SerializeField, Tooltip("文字列")]
        private string _word;

        [SerializeField, Tooltip("難易度"), Min(1)]
        private int _difficulty;
    }
}