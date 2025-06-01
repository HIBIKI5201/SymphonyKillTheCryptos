using System;
using UnityEngine;

namespace Cryptos.Runtime
{
    public class MoveController : MonoBehaviour
    {
        [SerializeField, Range(0, 10)] private float _larpSpeed = 5f;
        
        private Animator _animator;
        private SymphonyAnimeManager _symphonyAnimeManager;
        
        private Vector2 _lastDir;
        private void Start()
        {
            _animator = GetComponent<Animator>();
            _symphonyAnimeManager = GetComponent<SymphonyAnimeManager>();
        }

        void Update()
        {
            Vector2 dir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            
            _lastDir = Vector2.Lerp(_lastDir, dir, Time.deltaTime * _larpSpeed);
            _symphonyAnimeManager.SetDirX(_lastDir.x);
            _symphonyAnimeManager.SetDirY(_lastDir.y);
            _symphonyAnimeManager.SetVelocity(_lastDir.magnitude);
            _symphonyAnimeManager.SetSprint(Input.GetKey(KeyCode.LeftShift));
        }
    }
}
