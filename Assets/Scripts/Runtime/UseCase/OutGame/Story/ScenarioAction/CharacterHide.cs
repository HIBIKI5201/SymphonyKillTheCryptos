using Cryptos.Runtime.Entity.Outgame.Story;
using System.Threading;
using System.Threading.Tasks;

namespace Cryptos.Runtime.UseCase
{
    public class CharacterHide : CharacterActionBase
    {
        public CharacterHide() : base(string.Empty) { }
        public CharacterHide(string name) : base(name) { }

        public override ValueTask ExecuteAsync(IScenarioActionRepository repository, IPauseSubject pauseHandler, CancellationToken token = default)
        {
            repository.CharacterRepository.Hide(_name);
            return default;
        }
    }
}
