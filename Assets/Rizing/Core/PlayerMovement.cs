using System;
using Rizing.Interface;
using Rizing.Singletons;
using UnityEngine;

namespace Rizing.Core {
    public class PlayerMovement : MonoBehaviour, IEntity {
        [Header("Movement stuffs")]
        [SerializeField] private float moveSpeed = 5.5f;
        [SerializeField] private float jumpPower = 75;
        [Space]
        [SerializeField] private float dampCoefficent = 0.05f;
        [Space]
        [SerializeField] private float MaxAirForce = 10;
        [SerializeField] private float AirStrafeForce = 3;
        [Header("Misc")]
        [SerializeField] private GameObject moveVisualization;
        
        private Transform _fpsCamera;
        private Rigidbody _rigidbody;
        private InputParser _inputParser;

        private Vector3 _direction;
        private bool _grounded;
        private bool _readyJump = true;
        private float _groundedTicksDelta;

        private Vector3 _moveInput;
        private bool _jumpInput;
        
        private void Start() {
            _fpsCamera = FPSCamera.Instance.transform;
            _rigidbody = GetComponent<Rigidbody>();
            _inputParser = InputParser.Instance;
            
            GameManager.Instance.AddEntity(this);
        }

        public void Play() {
            
        }

        public void Pause() {
            
        }

        public void Process(float deltaTime) {
            _moveInput = _inputParser.GetKey("Move").ReadValue<Vector2>();
            _jumpInput = _inputParser.GetKey("Jump").IsPressed();

            var playerTransform = transform;
            _grounded = Physics.SphereCast(playerTransform.position, 0.5f, Vector3.down, out var hitted, 0.6f, ~LayerMask.GetMask("Player"));

            if (!_grounded) _readyJump = true;

            if (_grounded && !_readyJump) {
                _groundedTicksDelta += deltaTime;
                if (_groundedTicksDelta > 0.2f) {
                    _readyJump = true;
                    _groundedTicksDelta = 0;
                }
            }
            
            if (_jumpInput && _grounded && _readyJump) {
                var velocity = _rigidbody.velocity;
                velocity.y = 0;
                _rigidbody.velocity = velocity;
                
                _rigidbody.AddForce(transform.up * jumpPower, ForceMode.Impulse);
                _readyJump = false;
            }
        }

        public void FixedProcess(float deltaTime) {
            moveVisualization.transform.localPosition = _moveInput * 10;
            
            if (_grounded && _readyJump) {
                ProcessGroundedMovement(deltaTime);
            } else {
                ProcessAirMovement();
            }
        }

        private void ProcessGroundedMovement(float dt) {
            var forward = _fpsCamera.forward;
            forward.y = 0;
            
            _direction = forward * _moveInput.y + _fpsCamera.right * _moveInput.x;
            _direction.y = 0;
            
            //_rigidbody.AddForce(_direction * moveSpeed, ForceMode.Impulse);
            
            var velocity = _rigidbody.velocity + _direction.normalized * (moveSpeed * dt);

            float velY = velocity.y;
            var newVelocity = Vector3.Lerp(velocity, Vector3.zero, dampCoefficent);
            newVelocity.y = velY;
            
            _rigidbody.velocity = newVelocity;
        }
        
        //https://github.com/id-Software/Quake/blob/master/WinQuake/sv_user.c
        //i like quake movement, ok?
        private void ProcessAirMovement() {
            _direction = _fpsCamera.forward * _moveInput.y + _fpsCamera.right * _moveInput.x;
            _direction.y = 0;
            
            Vector3 wish_dir = _direction.normalized;

            float current_speed = Vector3.Dot(_rigidbody.velocity, wish_dir);
            float add_speed = Mathf.Clamp(AirStrafeForce - current_speed, 0, MaxAirForce);
            
            _rigidbody.AddForce(wish_dir * add_speed, ForceMode.Impulse);
        }

        public void LateProcess(float deltaTime) {
            
        }

        public void LateFixedProcess(float deltaTime) {
            
        }
    }
}