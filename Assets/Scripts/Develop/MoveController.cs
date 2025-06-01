using System;
using UnityEngine;

namespace Cryptos.Runtime
{
    public class MoveController : MonoBehaviour
    {
        private readonly int MoveX = Animator.StringToHash("MoveX");
        private readonly int MoveY = Animator.StringToHash("MoveY");
        private readonly int Velocity = Animator.StringToHash("Velocity");
        
        [SerializeField, Range(0, 10)] private float _larpSpeed = 5f;
        
        private Animator _animator;
        
        private Vector2 _lastDir;
        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        void Update()
        {
            Vector2 dir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            
            _lastDir = Vector2.Lerp(_lastDir, dir, Time.deltaTime * _larpSpeed);
            
            _animator.SetFloat(MoveX, _lastDir.x);
            _animator.SetFloat(MoveY, _lastDir.y);
            _animator.SetFloat(Velocity, _lastDir.magnitude);
        }
    }
}
