namespace Cryptos.Runtime.Entity.Ingame.Character
{
    /// <summary>
    /// 被弾可能なオブジェクトの静的なステータスデータを定義するインターフェースです。
    /// </summary>
    public interface IHittableData
    {
        /// <summary>
        /// 最大体力を取得します。
        /// </summary>
        public float MaxHealth { get; }

        /// <summary>
        /// 防御力を取得します（割合軽減）。
        /// </summary>
        public float Armor { get; }
    }
}
