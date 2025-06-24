using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cryptos.Runtime.Ingame.Entity
{
    public class WordManager
    {
        private HashSet<WordData> _existWords = new();

        /// <summary>
        ///     デッキにない唯一のワードを取得する
        /// </summary>
        /// <param name="candidateWords">候補となるワード</param>
        /// <returns></returns>
        public WordData GetOnlyWord(WordData[] candidateWords)
        {
            //既にデッキ上に含まれているものを除外
            IEnumerable<WordData> onlyWords = candidateWords
                .Where(word => !_existWords.Contains(word));

            if (onlyWords.Any()) //残っていたらランダムなワードを返す
            {
                int random = Random.Range(0, onlyWords.Count());
                return onlyWords.ElementAt(random);
            }
            else
            {
                Debug.LogError("候補ワードがありません");
                return null;
            }
        }

        public void AddWord(WordData word) => _existWords.Add(word);
        public void RemoveWord(WordData word) => _existWords.Remove(word);
    }
}