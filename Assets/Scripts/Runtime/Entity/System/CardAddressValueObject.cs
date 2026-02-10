using System;

namespace Cryptos.Runtime.Entity
{
    [Serializable]
    public readonly struct CardAddressValueObject
    {
        public CardAddressValueObject(string value) => _value = value;

        public string Value => _value;

        public bool Equals(CardAddressValueObject other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is CardAddressValueObject other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value != null ? Value.GetHashCode() : 0;
        }

        private readonly string _value;
    }
}
