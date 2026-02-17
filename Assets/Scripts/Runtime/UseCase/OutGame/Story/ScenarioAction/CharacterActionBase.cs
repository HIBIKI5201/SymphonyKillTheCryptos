using Cryptos.Runtime.Entity.Outgame.Story;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Cryptos.Runtime.UseCase
{
    public abstract class CharacterActionBase : IScenarioAction
    {
        public CharacterActionBase(string name)
        {
            _name = name;
        }

        public abstract ValueTask ExecuteAsync(IScenarioActionRepository repository,
            IPauseSubject pauseHandler,
            CancellationToken token = default);

        [SerializeField]
        protected string _name;
    }
}
