using System.Collections;
using System.Collections.Generic;

namespace Cryptos.Runtime
{
    public readonly struct ScenarioArguments : IEnumerable<string>
    {
        public ScenarioArguments(string[] args)
        {
            _args = args;
        }

        public Argument this[int index] => new(_args[index]);
        public int Length => _args.Length;

        public IEnumerator<string> GetEnumerator() => ((IEnumerable<string>)_args).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString() => string.Join(", ", this);

        private readonly string[] _args;

        public readonly struct Argument
        {
            public Argument(string value)
            {
                _value = value;
            }

            public static implicit operator string(Argument arg) => arg._value;
            public static implicit operator int(Argument arg)
            {
                if (int.TryParse(arg._value, out int result))
                {
                    return result;
                }
                return default;
            }

            public static implicit operator float(Argument arg)
            {
                if (float.TryParse(arg._value, out float result))
                {
                    return result;
                }
                return default;
            }

            private readonly string _value;
        }
    }
}
