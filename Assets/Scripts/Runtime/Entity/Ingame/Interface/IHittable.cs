namespace Cryptos.Runtime.Entity.Ingame.Character
{
    /// <summary>
    /// 攻撃を受けられるオブジェクトのインターフェースです。
    /// </summary>
    public interface IHittable
    {
        /// <summary>
        /// 被弾可能なオブジェクトの静的データを取得します。
        /// </summary>
        public IHittableData HittableData { get; }

        /// <summary>
        /// ダメージを受けます。
        /// </summary>
        /// <param name="damage">ダメージ情報を含むCombatContext。</param>
        public void AddHealthDamage(CombatContext damage);

        /// <summary>
        /// 体力を回復します。
        /// </summary>
        /// <param name="amount">回復量。</param>
        public void AddHealthHeal(float amount);

        /// <summary>
        /// 死亡処理を実行します。
        /// </summary>
        public void Dead();
    }
}
