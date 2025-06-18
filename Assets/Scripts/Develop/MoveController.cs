using System;
using UnityEngine;

namespace Cryptos.Runtime
{
    [RequireComponent(typeof(SymphonyAnimeManager))]
    public class MoveController : MonoBehaviour
    {
        [SerializeField, Range(0, 10)] private float _larpSpeed = 5f;
        
        private SymphonyAnimeManager _symphonyAnimeManager;
        
        private Vector2 _lastDir;
        private void Start()
        {
            _symphonyAnimeManager = GetComponent<SymphonyAnimeManager>();
        }

        void Update()
        {
            if (_symphonyAnimeManager == null) return;

            Vector2 dir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            
            _lastDir = Vector2.Lerp(_lastDir, dir, Time.deltaTime * _larpSpeed);
            _symphonyAnimeManager.SetDirX(_lastDir.x);
            _symphonyAnimeManager.SetDirY(_lastDir.y);
            _symphonyAnimeManager.SetVelocity(_lastDir.magnitude);
            _symphonyAnimeManager.SetSprint(Input.GetKey(KeyCode.LeftShift));
        }
    }
}
