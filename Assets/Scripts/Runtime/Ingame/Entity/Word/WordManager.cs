using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Cryptos.Runtime.Ingame.Entity
{
    /// <summary>
    ///     デッキのワードを管理するクラス
    /// </summary>
    public class WordManager
    {
        private HashSet<string> _existWords = new();

        /// <summary>
        ///     デッキにない唯一のワードを取得する
        /// </summary>
        /// <param name="candidateWordDatas">候補となるワード</param>
        /// <returns></returns>
        public (int difficulty, string word) GetAvailableWord(IEnumerable<WordData> candidateWordDatas)
        {
            (int difficulty, IEnumerable<string> words)[] dataArray =
                new (int, IEnumerable<string>)[candidateWordDatas.Count()];

            //各ワードデータから条件外のものを除外
            for (int i = 0; i < candidateWordDatas.Count(); i++)
            {
                WordData data = candidateWordDatas.ElementAt(i);
                IEnumerable<string> candidateWords = Array.Empty<string>();

                //除外処理
                candidateWords = GetOnlyWords(data.Words);
                candidateWords = GetNonStartWithWords(candidateWords);

                dataArray[i] = (data.Difficulty, candidateWords);
            }

            //中身をシャッフルする
            int n = dataArray.Length;
            Random rand = new();
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                // 要素を入れ替え
                var temp = dataArray[k];
                dataArray[k] = dataArray[n];
                dataArray[n] = temp;
            }

            foreach (var data in dataArray)
            {
                if (!data.words.Any()) continue; //中身が無ければ次にする 

                //ランダムなワードを出す
                int random = UnityEngine.Random.Range(0, data.words.Count());
                return (data.difficulty, data.words.ElementAt(random));
            }

            Debug.LogError("候補ワードがありません");
            return (-1, null);
        }

        public void AddWord(string word) => _existWords.Add(word);
        public void RemoveWord(string word) => _existWords.Remove(word);

        private IEnumerable<string> GetOnlyWords(IEnumerable<string> wordDatas)
        {
            return wordDatas.Where(word => !_existWords.Contains(word));
        }

        /// <summary>
        ///     文字列の最初が被っていないワードのみに厳選する
        /// </summary>
        /// <param name="wordDatas"></param>
        /// <returns></returns>
        private IEnumerable<string> GetNonStartWithWords(IEnumerable<string> wordDatas)
        {
            List<string> result = new();

            foreach (var candidate in wordDatas)
            {
                bool isConflicting = false;

                foreach (var exist in _existWords) //全てのワードと被らないか確認
                {
                    if (candidate.StartsWith(exist) || exist.StartsWith(candidate))
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