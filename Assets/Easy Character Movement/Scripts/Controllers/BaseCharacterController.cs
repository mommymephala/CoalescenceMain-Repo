using ECM.Components;
using ECM.Helpers;
using HorrorEngine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ECM.Controllers
{
    public class BaseCharacterController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float _speed = 5.0f;
        [SerializeField] private float _angularSpeed = 540.0f;
        [SerializeField] private float _acceleration = 50.0f;
        [SerializeField] private float _deceleration = 20.0f;
        [SerializeField] private float _groundFriction = 8f;
        [SerializeField] private bool _useBrakingFriction;
        [SerializeField] private float _brakingFriction = 8f;
        [SerializeField] private float _airFriction;
        [SerializeField] private float _airControl = 0.2f;

        [Header("Jump")]
        [SerializeField] private float _baseJumpHeight = 1.5f;
        [SerializeField] private float _jumpPreGroundedToleranceTime = 0.15f;
        [SerializeField] private float _jumpPostGroundedToleranceTime = 0.15f;

        //Private fields
        protected IPlayerInput _input;
        private Vector3 _moveDirection;
        protected bool _canJump = true;
        protected bool _jump;
        protected bool _isJumping;
        protected bool _updateJumpTimer;
        protected float _jumpTimer;
        protected float _jumpUngroundedTimer;
        private bool _allowVerticalMovement;
        private bool _restoreVelocityOnResume = true;

        public CharacterMovement movement { get; private set; }
        public Animator animator { get; set; }
        public bool allowVerticalMovement
        {
            get { return _allowVerticalMovement; }
            set
            {
                _allowVerticalMovement = value;
                if (movement) movement.useGravity = !_allowVerticalMovement;
            }
        }
        public float speed
        {
            get { return _speed; }
            set { _speed = Mathf.Max(0.0f, value); }
        }
        public float angularSpeed
        {
            get { return _angularSpeed; }
            set { _angularSpeed = Mathf.Max(0.0f, value); }
        }
        public float acceleration
        {
            get { return movement.isGrounded ? _acceleration : _acceleration * _airControl; }
            set { _acceleration = Mathf.Max(0.0f, value); }
        }
        public float deceleration
        {
            get { return movement.isGrounded ? _deceleration : _deceleration * _airControl; }
            set { _deceleration = Mathf.Max(0.0f, value); }
        }
        public float groundFriction
        {
            get { return _groundFriction; }
            set { _groundFriction = Mathf.Max(0.0f, value); }
        }
        public bool useBrakingFriction
        {
            get { return _useBrakingFriction; }
            set { _useBrakingFriction = value; }
        }
        public float brakingFriction
        {
            get { return _brakingFriction; }
            set { _brakingFriction = Mathf.Max(0.0f, value); }
        }
        public float airFriction
        {
            get { return _airFriction; }
            set { _airFriction = Mathf.Max(0.0f, value); }
        }
        public float airControl
        {
            get { return _airControl; }
            set { _airControl = Mathf.Clamp01(value); }
        }
        public float baseJumpHeight
        {
            get { return _baseJumpHeight; }
            set { _baseJumpHeight = Mathf.Max(0.0f, value); }
        }
        public float jumpImpulse
        {
            get { return Mathf.Sqrt(2.0f * _baseJumpHeight * movement.gravity.magnitude); }
        }
        public float jumpPreGroundedToleranceTime
        {
            get { return _jumpPreGroundedToleranceTime; }
            set { _jumpPreGroundedToleranceTime = Mathf.Max(value, 0.0f); }
        }
        public float jumpPostGroundedToleranceTime
        {
            get { return _jumpPostGroundedToleranceTime; }
            set { _jumpPostGroundedToleranceTime = Mathf.Max(value, 0.0f); }
        }
        public bool jump
        {
            get { return _jump; }
            set
            {
                if (_jump && !value) _canJump = true;
                _jump = value;
            }
        }
        public bool isJumping
        {
            get { return _isJumping; }
        }
        public bool isFalling
        {
            get { return !movement.isGrounded && movement.velocity.y < 0.0001f; }
        }
        public bool isGrounded
        {
            get { return movement.isGrounded; }
        }
        public Vector3 moveDirection
        {
            get { return _moveDirection; }
            set { _moveDirection = Vector3.ClampMagnitude(value, 1.0f); }
        }
        public bool pause { get; set; }
        public bool isPaused { get; private set; }
        public bool restoreVelocityOnResume
        {
            get { return _restoreVelocityOnResume; }
            set { _restoreVelocityOnResume = value; }
        }

        public virtual void OnValidate()
        {
            speed = _speed;
            angularSpeed = _angularSpeed;
            acceleration = _acceleration;
            deceleration = _deceleration;
            groundFriction = _groundFriction;
            brakingFriction = _brakingFriction;
            airFriction = _airFriction;
            airControl = _airControl;
            baseJumpHeight = _baseJumpHeight;
            jumpPreGroundedToleranceTime = _jumpPreGroundedToleranceTime;
            jumpPostGroundedToleranceTime = _jumpPostGroundedToleranceTime;
        }

        public virtual void Awake()
        {
            _input = GetComponent<IPlayerInput>();
            movement = GetComponent<CharacterMovement>();
            movement.platformUpdatesRotation = true;
            animator = GetComponentInChildren<Animator>();
        }

        public virtual void FixedUpdate()
        {
            Pause();
            if (isPaused) return;
            
            Move();
        }

        public virtual void Update()
        {
            HandleInput();
            if (isPaused) return;
            UpdateRotation();
            Animate();
        }

        private void Pause()
        {
            if (pause && !isPaused)
            {
                movement.Pause(true);
                isPaused = true;
            }
            else if (!pause && isPaused)
            {
                movement.Pause(false, restoreVelocityOnResume);
                isPaused = false;
            }
        }

        public void RotateTowards(Vector3 direction, bool onlyLateral = true)
        {
            movement.Rotate(direction, angularSpeed, onlyLateral);
        }

        public void RotateTowardsMoveDirection(bool onlyLateral = true)
        {
            RotateTowards(moveDirection, onlyLateral);
        }

        public void RotateTowardsVelocity(bool onlyLateral = true)
        {
            RotateTowards(movement.velocity, onlyLateral);
        }

        protected virtual void Jump()
        {
            if ((_jump && _canJump) && 
                (movement.isGrounded || _jumpUngroundedTimer < _jumpPostGroundedToleranceTime))
            {
                movement.ApplyVerticalImpulse(jumpImpulse);
        
                _isJumping = true;
                _canJump = false;
        
                _jumpUngroundedTimer = 0.0f;
            }

            if (!movement.isGrounded && _isJumping)
            {
                _jumpUngroundedTimer += Time.fixedDeltaTime;
            }

            if (movement.isGrounded && _isJumping && _jumpUngroundedTimer > 0)
            {
                _isJumping = false;
                _canJump = true;
            }
        }

        protected virtual void UpdateJumpTimer()
        {
            if (!_updateJumpTimer)
                return;

            if (!_jump) return;
            _jumpTimer = 0.0f;
            _updateJumpTimer = false;
        }

        protected virtual Vector3 CalcDesiredVelocity()
        {
            return moveDirection * speed;
        }

        protected virtual void Move()
        {
            var desiredVelocity = CalcDesiredVelocity();

            var currentFriction = isGrounded ? groundFriction : airFriction;
            var currentBrakingFriction = useBrakingFriction ? brakingFriction : currentFriction;

            movement.Move(desiredVelocity, speed, acceleration, deceleration, currentFriction,
                currentBrakingFriction, !allowVerticalMovement);
            
            Jump();
            UpdateJumpTimer();
        }

        protected virtual void Animate() { }

        protected virtual void UpdateRotation()
        {
            RotateTowardsMoveDirection();
        }

        protected virtual void HandleInput() { }
    }
}