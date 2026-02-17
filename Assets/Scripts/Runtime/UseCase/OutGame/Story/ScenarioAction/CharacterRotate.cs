using Cryptos.Runtime.Entity.Outgame.Story;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting.YamlDotNet.Serialization.ObjectGraphVisitors;
using UnityEngine;

namespace Cryptos.Runtime.UseCase
{
    public class CharacterRotate : CharacterActionBase
    {
        public CharacterRotate() : base(string.Empty) { }

        public CharacterRotate(string name, float degree) : base(name)
        {
            _degree = degree;
        }

        public override ValueTask ExecuteAsync(IScenarioActionRepository repository, IPauseSubject pauseHandler, CancellationToken token = default)
        {
            repository.CharacterRepository.Rotate(_name, _degree, 0);
            return default;
        }

        [SerializeField]
        private float _degree;
    }
}
