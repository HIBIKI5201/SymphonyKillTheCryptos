using Cryptos.Runtime.Entity.Outgame.Story;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Cryptos.Runtime.UseCase
{
    public class CameraChange : IScenarioAction
    {
        public CameraChange() 
        {
            _targetName = string.Empty;
        }

        public CameraChange(string target)
        {
            _targetName = target;
        }

        public async ValueTask ExecuteAsync(IScenarioActionRepository repository, IPauseSubject pauseHandler, CancellationToken token = default)
        {
            repository.CameraRepository.SetActiveCamera(_targetName);
        }

        [SerializeField]
        private string _targetName;
    }
}
