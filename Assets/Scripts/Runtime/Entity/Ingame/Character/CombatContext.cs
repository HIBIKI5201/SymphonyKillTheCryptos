namespace Cryptos.Runtime.Entity.Ingame.Character
{
    public struct CombatContext
    {
        public IAttackableData AttackerData => _attackerData;
        public IHittableData TargetData => _targetData;

        public float Damage => _damage;

        public CombatContext(IAttackableData attackerData, IHittableData targetData, float damage)
        {
            _attackerData = attackerData;
            _targetData = targetData;

            _damage = damage;
        }

        private IAttackableData _attackerData;
        private IHittableData _targetData;

        private float _damage;
    }
}
