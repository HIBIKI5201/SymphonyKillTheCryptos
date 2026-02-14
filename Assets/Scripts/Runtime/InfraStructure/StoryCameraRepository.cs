using Cryptos.Runtime.UseCase;
using Unity.Cinemachine;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.OutGame.Story
{
    public class StoryCameraRepository : MonoBehaviour, ICameraRepository
    {
        public CinemachineCamera this[int index] => _cameras[index];

        public void SetActiveCamera(int index)
        {
            CinemachineCamera camera = _cameras[index];
            camera.Prioritize();
        }

        [SerializeField]
        private CinemachineCamera[] _cameras;
    }
}
