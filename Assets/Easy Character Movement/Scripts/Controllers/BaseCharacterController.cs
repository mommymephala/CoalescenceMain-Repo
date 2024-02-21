using ECM.Components;
using ECM.Helpers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ECM.Controllers
{
    public class BaseCharacterController : MonoBehaviour
    {
        [Header("Inputs")]
        [SerializeField] private InputAction moveAction;
        [SerializeField] private InputAction jumpAction;
        [SerializeField] private InputAction sprintAction;

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

        [Header("Crouch")]
        [SerializeField] private bool _canCrouch = true;
        [SerializeField] private float _standingHeight = 2.0f;
        [SerializeField] private float _crouchingHeight = 1.0f;

        [Header("Jump")]
        [SerializeField] private float _baseJumpHeight = 1.5f;
        [SerializeField] private float _extraJumpTime = 0.5f;
        [SerializeField] private float _extraJumpPower = 25.0f;
        [SerializeField] private float _jumpPreGroundedToleranceTime = 0.15f;
        [SerializeField] private float _jumpPostGroundedToleranceTime = 0.15f;
        [SerializeField] private float _maxMidAirJumps = 1;

        [Header("Animation")]
        [SerializeField] private bool _useRootMotion;
        [SerializeField] private bool _rootMotionRotation;

        private Vector3 _moveDirection;
        protected bool _canJump = true;
        protected bool _jump;
        protected bool _isJumping;
        protected bool _updateJumpTimer;
        protected float _jumpTimer;
        protected float _jumpButtonHeldDownTimer;
        protected float _jumpUngroundedTimer;
        protected int _midAirJumpCount;
        private bool _allowVerticalMovement;
        private bool _restoreVelocityOnResume = true;

        public CharacterMovement movement { get; private set; }
        public Animator animator { get; set; }
        public RootMotionController rootMotionController { get; set; }
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
        public bool canCrouch
        {
            get { return _canCrouch; }
            set { _canCrouch = value; }
        }
        public float standingHeight
        {
            get { return _standingHeight; }
            set { _standingHeight = Mathf.Max(0.0f, value); }
        }
        public float crouchingHeight
        {
            get { return _crouchingHeight; }
            set { _crouchingHeight = Mathf.Max(0.0f, value); }
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
        public float extraJumpTime
        {
            get { return _extraJumpTime; }
            set { _extraJumpTime = Mathf.Max(0.0f, value); }
        }
        public float extraJumpPower
        {
            get { return _extraJumpPower; }
            set { _extraJumpPower = Mathf.Max(0.0f, value); }
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
        public float maxMidAirJumps
        {
            get { return _maxMidAirJumps; }
            set { _maxMidAirJumps = Mathf.Max(0.0f, value); }
        }
        public bool useRootMotion
        {
            get { return _useRootMotion; }
            set { _useRootMotion = value; }
        }
        public bool useRootMotionRotation
        {
            get { return _rootMotionRotation; }
            set { _rootMotionRotation = value; }
        }
        public bool applyRootMotion
        {
            get { return animator != null && animator.applyRootMotion; }
            set
            {
                if (animator != null) animator.applyRootMotion = value;
            }
        }
        public bool jump
        {
            get { return _jump; }
            set
            {
                if (_jump && !value) _canJump = true;
                _jump = value;
                if (_jump) _jumpButtonHeldDownTimer += Time.deltaTime;
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
        public bool crouch { get; set; }
        public bool isCrouching { get; protected set; }

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
            canCrouch = _canCrouch;
            crouchingHeight = _crouchingHeight;
            standingHeight = _standingHeight;
            baseJumpHeight = _baseJumpHeight;
            extraJumpTime = _extraJumpTime;
            extraJumpPower = _extraJumpPower;
            jumpPreGroundedToleranceTime = _jumpPreGroundedToleranceTime;
            jumpPostGroundedToleranceTime = _jumpPostGroundedToleranceTime;
            maxMidAirJumps = _maxMidAirJumps;
        }

        public virtual void Awake()
        {
            movement = GetComponent<CharacterMovement>();
            movement.platformUpdatesRotation = true;
            animator = GetComponentInChildren<Animator>();
            rootMotionController = GetComponentInChildren<RootMotionController>();
        }
        
        private void OnEnable()
        {
            moveAction.Enable();
            jumpAction.Enable();
            sprintAction.Enable();
        }
        
        private void OnDisable()
        {
            moveAction.Disable();
            jumpAction.Disable();
            sprintAction.Disable();
        }

        public virtual void FixedUpdate()
        {
            Pause();
            if (isPaused) return;
            Move();
            Crouch();
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
            if (isJumping)
            {
                if (!movement.wasGrounded && movement.isGrounded)
                    _isJumping = false;
            }

            if (movement.isGrounded)
                _jumpUngroundedTimer = 0.0f;
            else
                _jumpUngroundedTimer += Time.deltaTime;

            if (!_jump || !_canJump)
                return;

            if (_jumpButtonHeldDownTimer > _jumpPreGroundedToleranceTime)
                return;

            if (!movement.isGrounded && _jumpUngroundedTimer > _jumpPostGroundedToleranceTime)
                return;

            _canJump = false;
            _isJumping = true;
            _updateJumpTimer = true;

            _jumpUngroundedTimer = _jumpPostGroundedToleranceTime;

            movement.ApplyVerticalImpulse(jumpImpulse);

            movement.DisableGrounding();
        }

        protected virtual void MidAirJump()
        {
            if (_midAirJumpCount > 0 && movement.isGrounded)
                _midAirJumpCount = 0;

            if (!_jump || !_canJump)
                return;

            if (movement.isGrounded)
                return;

            if (_midAirJumpCount >= _maxMidAirJumps)
                return;

            _midAirJumpCount++;
            _canJump = false;
            _isJumping = true;
            _updateJumpTimer = true;

            movement.ApplyVerticalImpulse(jumpImpulse);

            movement.DisableGrounding();
        }

        protected virtual void UpdateJumpTimer()
        {
            if (!_updateJumpTimer)
                return;

            if (_jump && _jumpTimer < _extraJumpTime)
            {
                var jumpProgress = _jumpTimer / _extraJumpTime;
                var proportionalJumpPower = Mathf.Lerp(_extraJumpPower, 0f, jumpProgress);
                movement.ApplyForce(transform.up * proportionalJumpPower, ForceMode.Acceleration);
                _jumpTimer = Mathf.Min(_jumpTimer + Time.deltaTime, _extraJumpTime);
            }
            else
            {
                _jumpTimer = 0.0f;
                _updateJumpTimer = false;
            }
        }

        protected virtual void Crouch()
        {
            if (!canCrouch)
                return;

            if (crouch)
            {
                if (isCrouching)
                    return;

                movement.SetCapsuleHeight(crouchingHeight);
                isCrouching = true;
            }
            else
            {
                if (!isCrouching)
                    return;

                if (!movement.ClearanceCheck(standingHeight))
                    return;

                movement.SetCapsuleHeight(standingHeight);
                isCrouching = false;
            }
        }

        protected virtual Vector3 CalcDesiredVelocity()
        {
            if (useRootMotion && applyRootMotion)
                return rootMotionController.animVelocity;

            return moveDirection * speed;
        }

        protected virtual void Move()
        {
            var desiredVelocity = CalcDesiredVelocity();

            if (useRootMotion && applyRootMotion)
                movement.Move(desiredVelocity, speed, !allowVerticalMovement);
            else
            {
                var currentFriction = isGrounded ? groundFriction : airFriction;
                var currentBrakingFriction = useBrakingFriction ? brakingFriction : currentFriction;

                movement.Move(desiredVelocity, speed, acceleration, deceleration, currentFriction,
                    currentBrakingFriction, !allowVerticalMovement);
            }

            Jump();
            MidAirJump();
            UpdateJumpTimer();

            applyRootMotion = useRootMotion && movement.isGrounded;
        }

        protected virtual void Animate() { }

        protected virtual void UpdateRotation()
        {
            if (useRootMotion && applyRootMotion && useRootMotionRotation)
            {
                movement.rotation *= animator.deltaRotation;
            }
            else
            {
                RotateTowardsMoveDirection();
            }
        }

        protected virtual void HandleInput()
        {
            moveDirection = new Vector3
            {
                x = Input.GetAxisRaw("Horizontal"),
                y = 0.0f,
                z = Input.GetAxisRaw("Vertical")
            };

            jump = Input.GetButton("Jump");

            crouch = Input.GetKey(KeyCode.C);
        }
    }
}