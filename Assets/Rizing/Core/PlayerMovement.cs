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
        [SerializeField] private float maxWalkAngle = 0.8f;
        [Space]
        [SerializeField] private float maxAirForce = 10;
        [SerializeField] private float airStrafeForce = 3;
        [Header("Misc")]
        [SerializeField] private GameObject moveVisualization;
        
        private Transform fpsCamera;
        private Rigidbody rigid;
        private InputParser inputParser;
        private Transform playerTransform;

        private Vector3 direction;
        private bool grounded;
        private bool readyJump = true;
        private float groundedTicksDelta;

        private Vector3 _moveInput;
        private bool _jumpInput;

        private float current_speed;
        private float add_speed;
        
        private void Start() {
            fpsCamera = FPSCamera.Instance.transform;
            rigid = GetComponent<Rigidbody>();
            inputParser = InputParser.Instance;

            playerTransform = transform;
            
            GameManager.Instance.AddEntity(this);
        }

        public void Play() {
            
        }

        public void Pause() {
            
        }

        public void Process(float deltaTime) {
            _moveInput = inputParser.GetKey("Move").ReadValue<Vector2>();
            _jumpInput = inputParser.GetKey("Jump").IsPressed();
            
            grounded = Physics.SphereCast(playerTransform.position, 0.5f, Vector3.down, out var hitted, 0.5f, ~LayerMask.GetMask("Player"));

            if (_jumpInput && grounded && readyJump && Vector3.Dot(hitted.normal, Vector3.up) > maxWalkAngle) {
                var velocity = rigid.linearVelocity;
                velocity.y = 0;
                rigid.linearVelocity = velocity;
                
                rigid.AddForce(transform.up * jumpPower, ForceMode.Impulse);
                readyJump = false;
            }
            if (!grounded) readyJump = true;
        }

        public void FixedProcess(float deltaTime) {
            moveVisualization.transform.localPosition = _moveInput * 10;
            
            Physics.SphereCast(playerTransform.position, 0.5f, Vector3.down, out var groundHit, 0.5f, ~LayerMask.GetMask("Player"));
            
            if (grounded) {
                if(Vector3.Dot(groundHit.normal, Vector3.up) < maxWalkAngle) {
                    grounded = false;
                }
            }
            
            if (grounded && readyJump) {
                ProcessGroundedMovement(deltaTime, groundHit);
            } else {
                ProcessAirMovement();
            }
        }

        private void ProcessGroundedMovement(float dt, RaycastHit groundHit) {
            var forward = fpsCamera.forward;
            forward.y = 0;
            
            direction = forward * _moveInput.y + fpsCamera.right * _moveInput.x;
            direction.y = 0;
            
            //_rigidbody.AddForce(_direction * moveSpeed, ForceMode.Impulse);
            
            var velocity = rigid.linearVelocity + direction.normalized * (moveSpeed * dt);

            float upDiff = Vector3.Dot(groundHit.normal, Vector3.up);

            Vector3 cross = Vector3.Cross(Vector3.up, forward);
            velocity = Quaternion.AngleAxis(upDiff, cross) * velocity;
            
            float velY = velocity.y;
            var newVelocity = Vector3.Lerp(velocity, Vector3.zero, dampCoefficent);
            newVelocity.y = velY;
            
            rigid.linearVelocity = newVelocity;
        }
        
        //https://github.com/id-Software/Quake/blob/master/WinQuake/sv_user.c
        //i like quake movement, ok?
        private void ProcessAirMovement() {
            direction = fpsCamera.forward * _moveInput.y + fpsCamera.right * _moveInput.x;
            direction.y = 0;
            
            Vector3 wish_dir = direction.normalized;

            current_speed = Vector3.Dot(rigid.linearVelocity, wish_dir);
            add_speed = Mathf.Clamp(airStrafeForce - current_speed, 0, maxAirForce);

            Vector3 vel = wish_dir * add_speed;
            
            rigid.AddForce(vel, ForceMode.Impulse);
        }

        public void LateProcess(float deltaTime) {
            
        }

        public void LateFixedProcess(float deltaTime) {
            
        }
    }
}