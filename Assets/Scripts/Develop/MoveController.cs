using System;
using UnityEngine;

namespace Cryptos.Runtime
{
    public class MoveController : MonoBehaviour
    {
        private readonly int MoveXHash = Animator.StringToHash("MoveX");
        private readonly int MoveYHash = Animator.StringToHash("MoveY");
        private readonly int VelocityHash = Animator.StringToHash("Velocity");
      private readonly int SprintHash = Animator.StringToHash("IsSprint");  
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
            
            _animator.SetFloat(MoveXHash, _lastDir.x);
            _animator.SetFloat(MoveYHash, _lastDir.y);
            _animator.SetFloat(VelocityHash, _lastDir.magnitude);
            _animator.SetBool(SprintHash, Input.GetKey(KeyCode.LeftShift));
        }
    }
}
