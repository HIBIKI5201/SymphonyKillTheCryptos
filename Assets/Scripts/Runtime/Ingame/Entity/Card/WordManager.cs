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
        public WordData GetAvailableWord(IEnumerable<WordData> candidateWords)
        {
            //既にデッキ上に含まれているものを除外

            candidateWords = GetOnlyWords(candidateWords);
            candidateWords = GetNonStartWithWords(candidateWords); 


            if (candidateWords.Any()) //残っていたらランダムなワードを返す
            {
                int random = Random.Range(0, candidateWords.Count());
                return candidateWords.ElementAt(random);
            }
            else
            {
                Debug.LogError("候補ワードがありません");
                return null;
            }
        }

        public void AddWord(WordData word) => _existWords.Add(word);
        public void RemoveWord(WordData word) => _existWords.Remove(word);

        private IEnumerable<WordData> GetOnlyWords(IEnumerable<WordData> wordDatas)
        {
            return wordDatas.Where(word => !_existWords.Contains(word));
        }

        /// <summary>
        ///     文字列の最初が被っていないワードのみに厳選する
        /// </summary>
        /// <param name="wordDatas"></param>
        /// <returns></returns>
        private IEnumerable<WordData> GetNonStartWithWords(IEnumerable<WordData> wordDatas)
        {
            List<WordData> result = new();

            foreach (var candidate in wordDatas)
            {
                bool isConflicting = false;

                foreach (var exist in _existWords) //全てのワードと被らないか確認
                {
                    if (candidate.Word.StartsWith(exist.Word) || exist.Word.StartsWith(candidate.Word))
                    {
                        isConflicting = true;
                        break;
                    }
                }

                if (!isConflicting) //もし一つも被らなければ候補に追加
                {
                    result.Add(candidate);
                }
            }

            return result;
        }
    }
}