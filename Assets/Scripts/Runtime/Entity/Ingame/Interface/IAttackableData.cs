namespace Cryptos.Runtime.Entity.Ingame.Character
{
    public interface IAttackableData
    {
        public float AttackPower { get; }

        public float CriticalChance { get; }
        public float CriticalDamage { get; }
    }
}
