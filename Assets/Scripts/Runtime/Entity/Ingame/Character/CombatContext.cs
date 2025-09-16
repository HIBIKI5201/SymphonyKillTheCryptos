namespace Cryptos.Runtime.Entity.Ingame.Character
{
    /// <summary>
    /// 一回の戦闘イベントにおける全ての情報を保持する構造体です。
    /// この構造体は、戦闘処理のハンドラー間で受け渡されます。
    /// </summary>
    public readonly struct CombatContext
    {
        /// <summary>
        ///     CombatContextの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="attackerData">攻撃側の静的データ。</param>
        /// <param name="targetData">防御側の静的データ。</param>
        /// <param name="damage">初期ダメージ量。</param>
        public CombatContext(IAttackableData attackerData, IHittableData targetData,
            float damage, int criticalCount = 0)
        {
            _attackerData = attackerData;
            _targetData = targetData;

            _damage = damage;
            _criticalCount = criticalCount;
            _element = string.Empty;
        }

        /// <summary>
        ///     元のデータからダメージだけを変更します
        /// </summary>
        /// <param name="original"></param>
        /// <param name="damage"></param>
        public CombatContext(CombatContext original, float damage)
        {
            _attackerData = original._attackerData;
            _targetData = original.TargetData;

            _damage = damage;
            _criticalCount = original.CriticalCount;
            _element = string.Empty;
        }

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
        /// クリティカルの重複数を取得します。
        /// </summary>
        public int CriticalCount => _criticalCount;

        /// <summary>
        /// 属性を取得します。
        /// </summary>
        public string Element => _element;

        private readonly IAttackableData _attackerData;
        private readonly IHittableData _targetData;

        private readonly float _damage;
        private readonly int _criticalCount;
        private readonly string _element;
    }
}
