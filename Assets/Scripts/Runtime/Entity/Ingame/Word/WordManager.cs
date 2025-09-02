using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Cryptos.Runtime.Entity.Ingame.Word
{
    /// <summary>
    /// ゲーム内で使用されるワードの重複や競合を管理するクラスです。
    /// 既に存在するワードや、先頭文字が競合するワードを除外して、利用可能なワードを提供します。
    /// </summary>
    public class WordManager
    {
        /// <summary>使用済みのワードを格納するHashSet</summary>
        private readonly HashSet<string> _existWords = new();

        /// <summary>
        /// 利用可能なワードの中から、デッキにない唯一のワードを一つ取得します。
        /// 候補となるワードデータの中から、既に使用されているワードや、他の使用中ワードと前方一致・後方一致するワードを除外した上で、
        /// ランダムに一つ選択して返します。
        /// </summary>
        /// <param name="candidateWordDatas">候補となるWordDataの配列</param>
        /// <returns>選択されたワードの難易度とワード本体のタプル。利用可能なワードがない場合は(-1, null)を返します。</returns>
        public (int difficulty, string word) GetAvailableWord(WordData[] candidateWordDatas)
        {
            // 難易度とフィルタリングされたワードのリストを格納する配列
            (int difficulty, IEnumerable<string> words)[] dataArray =
                new (int, IEnumerable<string>)[candidateWordDatas.Length];

            for (int i = 0; i < candidateWordDatas.Length; i++)
            {
                WordData data = candidateWordDatas[i];
                IEnumerable<string> candidateWords;

                // 未使用のワードのみを抽出
                candidateWords = GetOnlyWords(data.Words);
                // 他のワードと競合しないワードのみを抽出
                candidateWords = GetNonStartWithWords(candidateWords);

                dataArray[i] = (data.Difficulty, candidateWords);
            }

            // 難易度をシャッフル（フィッシャー・イェーツ法）
            Random rand = new();
            for (int i = dataArray.Length - 1; i > 0; i--)
            {
                int k = rand.Next(0, i + 1);
                (dataArray[k], dataArray[i]) = (dataArray[i], dataArray[k]);
            }

            // シャッフルされた候補の中から最初に見つかった有効なワードを返す
            foreach (var data in dataArray)
            {
                if (!data.words.Any()) continue;

                var wordList = data.words.ToList();
                int random = UnityEngine.Random.Range(0, wordList.Count);
                return (data.difficulty, wordList[random]);
            }

            Debug.LogError("利用可能な候補ワードがありませんでした。");
            return (-1, null);
        }

        /// <summary>
        /// 指定されたワードを使用済みワードのセットに追加します。
        /// </summary>
        /// <param name="word">追加するワード</param>
        public void AddWord(string word) => _existWords.Add(word);

        /// <summary>
        /// 指定されたワードを使用済みワードのセットから削除します。
        /// </summary>
        /// <param name="word">削除するワード</param>
        public void RemoveWord(string word) => _existWords.Remove(word);

        /// <summary>
        /// 既に使用済みのワードを除外したワードのコレクションを返します。
        /// </summary>
        /// <param name="wordDatas">フィルタリング対象のワードコレクション</param>
        /// <returns>未使用のワードのコレクション</returns>
        private IEnumerable<string> GetOnlyWords(IEnumerable<string> wordDatas)
        {
            return wordDatas.Where(word => !_existWords.Contains(word));
        }

        /// <summary>
        /// 既に使用中のどのワードとも前方一致・後方一致しないワードのみをフィルタリングします。
        /// これにより、例えば "int" と "interface" のような、片方がもう片方を包括するワードが同時に出現するのを防ぎます。
        /// </summary>
        /// <param name="wordDatas">フィルタリング対象のワードコレクション</param>
        /// <returns>競合しないワードのコレクション</returns>
        private IEnumerable<string> GetNonStartWithWords(IEnumerable<string> wordDatas)
        {
            return wordDatas.Where(candidate => 
                _existWords.All(exist => !candidate.StartsWith(exist) && !exist.StartsWith(candidate)));
        }
    }
}
