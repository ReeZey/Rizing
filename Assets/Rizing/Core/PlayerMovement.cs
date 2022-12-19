using Rizing.Interface;
using Rizing.Singletons;
using UnityEngine;

namespace Rizing.Core {
    public class PlayerMovement : MonoBehaviour, IEntity {
        [SerializeField] private float moveSpeed = 10;
        [SerializeField] private float moveAcceleration = 10;
        [SerializeField] private float jumpPower = 8;
        [SerializeField] private float dampCoefficent = 0.2f;
        
        private Transform _fpsCamera;
        private Rigidbody _rigidbody;
        private InputParser _inputParser;

        private Vector3 _direction;
        private bool _grounded;
        private bool _readyJump = true;

        private Vector3 _moveInput;
        private bool _jumpInput;

        [SerializeField] private GameObject MoveVisualization;
        
        [SerializeField] private float MaxAirSpeed;
        [SerializeField] private float AirStrafeForce;

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

            /*
            if (_grounded && _readyJump) {
                Vector3 flatVel = new Vector3(velocity.x, 0f, velocity.z);
            
                if(flatVel.magnitude > moveSpeed)
                {
                    //Vector3 limitedVel = flatVel.normalized * moveSpeed;
                    //_rigidbody.velocity = new Vector3(limitedVel.x, velocity.y, limitedVel.z);

                    
                    //do something
                }
            }
            */
        }

        public void FixedProcess(float deltaTime) {
            MoveVisualization.transform.localPosition = _moveInput * 10;
            
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
            
            _rigidbody.AddForce(_direction * moveAcceleration, ForceMode.Impulse);

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

        /*
        private void OnDrawGizmosSelected() {
            if (!Application.isPlaying) return;
            
            Gizmos.color = Color.blue;
            var cameraPos = _fpsCamera.position;
            
            Gizmos.DrawLine(cameraPos, cameraPos + _direction.normalized);
            
            Gizmos.color = Color.red;

            var playerPos = transform.position;
            Gizmos.DrawLine(playerPos, playerPos + Vector3.down);
        }
        */
    }
}