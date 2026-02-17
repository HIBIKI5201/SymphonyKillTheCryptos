using UnityEngine;

namespace Cryptos.Runtime.Presenter
{
    public class StoryCharacterPresenter : MonoBehaviour
    {
        private Camera _camera;

        private void Start()
        {
            GetCamera();
        }

        void Update()
        {
            if (_camera == null) { GetCamera(); }

            Vector3 dir = _camera.transform.forward;
            dir.y = 0f;

            if (dir.sqrMagnitude < 0.0001f){ return; }
            dir.Normalize();

            transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }

        private void GetCamera()
        {
            _camera = Camera.main;
        }
    }
}
