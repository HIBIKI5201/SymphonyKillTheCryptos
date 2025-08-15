namespace Cryptos.Runtime.Entity.Ingame.Character
{
    public struct CombatContext
    {
        public IAttackableData AttackerData => _attackerData;
        public IHitableData TargetData => _targetData;

        public float Damage => _damage;

        public CombatContext(IAttackableData attackerData, IHitableData targetData, float damage)
        {
            _attackerData = attackerData;
            _targetData = targetData;

            _damage = damage;
        }

        private IAttackableData _attackerData;
        private IHitableData _targetData;

        private float _damage;
    }
}
