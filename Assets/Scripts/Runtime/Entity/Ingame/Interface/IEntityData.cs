using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptos.Runtime.Entity.Ingame.Character
{
    /// <summary>
    /// オブジェクトの静的なステータスデータを定義するインターフェースです。
    /// </summary>
    public interface IEntityData
    {
        /// <summary>
        /// オブジェクトの名前を取得します。
        /// </summary>
        public string Name { get; }
    }
}
