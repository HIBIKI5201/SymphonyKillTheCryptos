namespace Cryptos.Runtime.Entity.Ingame.Character
{
    public struct CombatContext
    {
        public IAttackableData AttackerData => _attackerData;
        public IHitableData TargetData => _targetData;

        public float Damage => _damage;

        public CombatContext(IAttackable attacker, IHitable target, float damage)
        {
            _attackerData = attacker.AttackableData;
            _targetData = target.HitableData;

            _damage = damage;
        }

        private IAttackableData _attackerData;
        private IHitableData _targetData;

        private float _damage;
    }
}
