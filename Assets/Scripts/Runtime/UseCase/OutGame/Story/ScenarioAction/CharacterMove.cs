using Cryptos.Runtime.Entity.Outgame.Story;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Cryptos.Runtime.UseCase
{
    public class CharacterMove : CharacterActionBase
    {
        public CharacterMove() : base(string.Empty) { }
        public CharacterMove(string name, float x, float y, float duration) : base(name)
        {
            _position = new(x, y);
            _duration = duration;
        }

        public override async ValueTask ExecuteAsync(IScenarioActionRepository repository, IPauseSubject pauseHandler, CancellationToken token = default)
        {
            await repository.CharacterRepository.Move(_name, _position, _duration);
        }

        [SerializeField]
        private Vector2 _position;
        [SerializeField]
        private float _duration;
    }
}
