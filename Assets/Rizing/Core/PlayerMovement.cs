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
        [SerializeField] private float MaxAirSpeed = 10;
        [SerializeField] private float AirStrafeForce = 2;
        [Header("Misc")]
        [SerializeField] private GameObject moveVisualization;
        
        private Transform _fpsCamera;
        private Rigidbody _rigidbody;
        private InputParser _inputParser;

        private Vector3 _direction;
        private bool _grounded;
        private bool _readyJump = true;

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
            _jumpInput = _inputParser.GetKey("Jump").WasPressedThisFrame();

            var playerTransform = transform;
            _grounded = Physics.SphereCast(playerTransform.position, 0.5f, Vector3.down, out var hitted, 0.6f, ~LayerMask.GetMask("Player"));

            if (!_grounded) _readyJump = true;

            var velocity = _rigidbody.velocity;
            if (_jumpInput && _grounded && _readyJump) {
                velocity.y = 0;
                
                _rigidbody.velocity = velocity;
                
                _rigidbody.AddForce(transform.up * jumpPower, ForceMode.Impulse);
                _readyJump = false;
            }
        }

        public void FixedProcess(float deltaTime) {
            moveVisualization.transform.localPosition = _moveInput * 10;
            
            if (_grounded) {
                ProcessGroundedMovement();
            } else {
                ProcessAirMovement();
            }
        }

        private void ProcessGroundedMovement() {
            var forward = _fpsCamera.forward;
            forward.y = 0;
            
            _direction = forward.normalized * _moveInput.y + _fpsCamera.right * _moveInput.x;
            _direction.y = 0;
            
            _rigidbody.AddForce(_direction * moveSpeed, ForceMode.Impulse);

            var velocity = _rigidbody.velocity;

            float velY = velocity.y;
            var newVelocity = Vector3.Lerp(velocity, Vector3.zero, dampCoefficent);
            newVelocity.y = velY;
            
            _rigidbody.velocity = newVelocity;
        }
        
        //https://www.reddit.com/r/Unity3D/comments/dcmvf5/i_replicated_the_source_engines_air_strafing/
        //slightly modified
        private void ProcessAirMovement() {
            _direction = _fpsCamera.forward * _moveInput.y + _fpsCamera.right * _moveInput.x;
            _direction.y = 0;
            
            var velocity = _rigidbody.velocity;
            
            Vector3 projVel = Vector3.Project(velocity, _direction);
            
            bool isAway = Vector3.Dot(_direction, projVel) <= 0f;
            
            if (Vector3.Dot(_direction.normalized, velocity.normalized) <= -0.8) {
                _rigidbody.velocity = Vector3.Lerp(new Vector3(0, velocity.y, 0), Vector3.zero, dampCoefficent);
                return;
            }

            if (!(projVel.magnitude < MaxAirSpeed) && !isAway) return;

            Vector3 vc = _direction.normalized * AirStrafeForce;
            
            vc = Vector3.ClampMagnitude(vc, isAway ? MaxAirSpeed + projVel.magnitude : MaxAirSpeed - projVel.magnitude);
            
            _rigidbody.AddForce(vc, ForceMode.Impulse);
        }

        public void LateProcess(float deltaTime) {
            
        }
    }
}