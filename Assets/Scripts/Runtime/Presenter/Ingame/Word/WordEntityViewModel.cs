using Cryptos.Runtime.Entity.Ingame.Word;
using System;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.Ingame.Word
{
    /// <summary>
    ///     WordEntityのビューモデル。
    /// </summary>
    public readonly struct WordEntityViewModel
    {
        /// <summary>
        ///     コンストラクタ。
        /// </summary>
        /// <param name="wordEntity">ラップするWordEntity。</param>
        public WordEntityViewModel(WordEntity wordEntity)
        {
            _entity = wordEntity;
        }

        /// <summary>
        ///     ワードが完成した時に発火するイベント。
        /// </summary>
        public event Action OnComplete
        {
            add => _entity.OnComplete += value;
            remove => _entity.OnComplete -= value;
        }

        /// <summary>
        ///     ワードが更新された時に発火するイベント。
        /// </summary>
        public event Action<string, int> OnWordUpdated
        {
            add => _entity.OnWordUpdated += value;
            remove => _entity.OnWordUpdated -= value;
        }

        /// <summary>
        ///     現在のワード。
        /// </summary>
        public string Word => _entity.CurrentWord;

        /// <summary>
        ///     文字を入力する。
        /// </summary>
        /// <param name="c">入力する文字。</param>
        public void InputChar(char c) => _entity?.OnInputChar(c);

        private readonly WordEntity _entity;
    }
}
