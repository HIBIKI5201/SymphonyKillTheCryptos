namespace Cryptos.Runtime.Entity
{
    public readonly struct CardAddressValueObject
    {
        public CardAddressValueObject(string value) => _value = value;

        public string Value => _value;

        private readonly string _value;
    }
}
