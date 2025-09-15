using System;
using UnityEngine;
using Cryptos.Runtime.Presenter.Ingame.Character.Player;

namespace Cryptos.Runtime.Develop
{
    [RequireComponent(typeof(ISymphonyAnimeManager))]
    public class MoveController : MonoBehaviour
    {
        [SerializeField, Range(0, 10)] private float _lerpSpeed = 5f;
        
        private ISymphonyAnimeManager _symphonyAnimeManager;
        
        private Vector2 _lastDir;
        private void Start()
        {
            _symphonyAnimeManager = GetComponent<ISymphonyAnimeManager>();
        }

        void Update()
        {
            if (_symphonyAnimeManager == null) return;

            HandleInput();
        }

        private void HandleInput()
        {
            Vector2 dir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            
            _lastDir = Vector2.Lerp(_lastDir, dir, Time.deltaTime * _lerpSpeed);
            _symphonyAnimeManager.SetDirX(_lastDir.x);
            _symphonyAnimeManager.SetDirY(_lastDir.y);
            _symphonyAnimeManager.SetVelocity(_lastDir.magnitude);
            _symphonyAnimeManager.SetSprint(Input.GetKey(KeyCode.LeftShift));
        }
    }
}
