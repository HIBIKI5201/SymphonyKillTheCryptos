using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Cryptos.Runtime.Entity.Ingame.Word
{
    /// <summary>
    /// ゲーム内で使用されるワードの重複や競合を管理するクラス。
    /// 既に存在するワードや、先頭文字が競合するワードを除外して、利用可能なワードを提供します。
    /// </summary>
    public class WordManager
    {
        private HashSet<string> _existWords = new();

        /// <summary>
        /// デッキにない唯一のワードを取得します。
        /// 既に存在するワードや、先頭文字が競合するワードを除外して、利用可能なワードを返します。
        /// </summary>
        /// <param name="candidateWordDatas">候補となるWordDataのコレクション</param>
        /// <returns>難易度とワードのタプル。利用可能なワードがない場合は(-1, null)。</returns>
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

        /// <summary>
        /// 指定されたワードを現在存在するワードのセットに追加します。
        /// </summary>
        /// <param name="word">追加するワード</param>
        public void AddWord(string word) => _existWords.Add(word);

        /// <summary>
        /// 指定されたワードを現在存在するワードのセットから削除します。
        /// </summary>
        /// <param name="word">削除するワード</param>
        public void RemoveWord(string word) => _existWords.Remove(word);

        /// <summary>
        /// 既に存在するワードのセットに含まれていないワードのみをフィルタリングします。
        /// </summary>
        /// <param name="wordDatas">フィルタリング対象のワードコレクション</param>
        /// <returns>重複しないワードのコレクション</returns>
        private IEnumerable<string> GetOnlyWords(IEnumerable<string> wordDatas)
        {
            return wordDatas.Where(word => !_existWords.Contains(word));
        }

        /// <summary>
        /// 既に存在するワードの先頭文字と競合しないワードのみをフィルタリングします。
        /// </summary>
        /// <param name="wordDatas">フィルタリング対象のワードコレクション</param>
        /// <returns>競合しないワードのコレクション</returns>
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
