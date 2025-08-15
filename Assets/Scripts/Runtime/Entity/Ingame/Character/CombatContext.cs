namespace Cryptos.Runtime.Entity.Ingame.Character
{
    /// <summary>
    /// 一回の戦闘イベントにおける全ての情報を保持する構造体です。
    /// この構造体は、戦闘処理のハンドラー間で受け渡されます。
    /// </summary>
    public struct CombatContext
    {
        /// <summary>
        /// 攻撃側の静的データを取得します。
        /// </summary>
        public IAttackableData AttackerData => _attackerData;

        /// <summary>
        /// 防御側の静的データを取得します。
        /// </summary>
        public IHittableData TargetData => _targetData;

        /// <summary>
        /// 現在の計算済みダメージ量を取得します。
        /// </summary>
        public float Damage => _damage;

        /// <summary>
        /// CombatContextの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="attackerData">攻撃側の静的データ。</param>
        /// <param name="targetData">防御側の静的データ。</param>
        /// <param name="damage">初期ダメージ量。</param>
        public CombatContext(IAttackableData attackerData, IHittableData targetData, float damage)
        {
            _attackerData = attackerData;
            _targetData = targetData;

            _damage = damage;
        }

        private readonly IAttackableData _attackerData;
        private readonly IHittableData _targetData;

        private readonly float _damage;
    }
}
