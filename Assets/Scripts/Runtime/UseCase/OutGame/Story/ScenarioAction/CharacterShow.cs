using Cryptos.Runtime.Entity.Outgame.Story;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Cryptos.Runtime.UseCase
{
    public class CharacterShow : CharacterActionBase
    {
        public CharacterShow() : base(string.Empty) { }

        public CharacterShow(string name, float x, float y) : base(name)
        {
            _position = new(x, y);
        }

        public override ValueTask ExecuteAsync(IScenarioActionRepository repository, IPauseSubject pauseHandler, CancellationToken token = default)
        {
            repository.CharacterRepository.Show(_name);
            repository.CharacterRepository.Move(_name, _position, 0f);

            return default;
        }

        [SerializeField]
        private Vector2 _position;
    }
}
