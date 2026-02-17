using Cryptos.Runtime.Entity.Outgame.Story;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

namespace Cryptos.Runtime.InfraStructure.OutGame.Story
{
    public class StoryCameraRepository : MonoBehaviour, ICameraRepository
    {
        public CinemachineCamera this[int index] => _cameras[index];

        public void SetActiveCamera(int index)
        {
            if (0 <= index && index < _cameras.Length)  
            {
                Debug.LogError($"{index} はCameraRepositoryの範囲外です。");
                return; 
            }

            CinemachineCamera camera = _cameras[index];
            camera.Prioritize();
        }

        public void SetActiveCamera(string name)
        {
            CinemachineCamera camera = _cameras.First(c => c.Name == name);
            if (camera == null) 
            {
                Debug.LogError($"{name} はCameraRepositoryに存在しません。");
                return;
            }

            camera.Prioritize();
        }

        [SerializeField]
        private CinemachineCamera[] _cameras;
    }
}
