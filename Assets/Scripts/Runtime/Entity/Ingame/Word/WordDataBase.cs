using Cryptos.Runtime.Framework;
using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Word
{
    /// <summary>
    /// ワードデータが格納されたScriptableObjectデータベース。
    /// ゲーム内で使用される全てのワードデータを管理します。
    /// </summary>
    [CreateAssetMenu(fileName = nameof(WordDataBase),
        menuName = CryptosPathConstant.ASSET_PATH + nameof(WordDataBase))]
    public class WordDataBase : ScriptableObject
    {
        /// <summary>
        /// 指定されたインデックスのWordDataから単語文字列の配列を取得します。
        /// </summary>
        /// <param name="index">WordDataのインデックス</param>
        /// <returns>単語文字列の配列</returns>
        public string[] this[int index] => _words[index].Words;

        /// <summary>
        /// 全てのWordDataの配列を取得します。
        /// </summary>
        public WordData[] WordData => _words;

        [SerializeField]
        private WordData[] _words;
    }
}
