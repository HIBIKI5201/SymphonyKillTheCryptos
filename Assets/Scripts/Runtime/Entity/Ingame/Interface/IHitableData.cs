namespace Cryptos.Runtime.Entity
{
    public interface IHitableData
    {
        public float Health { get; }
        public float MaxHealth { get; }

        public float Armor { get; }
    }
}
