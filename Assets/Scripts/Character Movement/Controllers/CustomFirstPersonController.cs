using Audio;
using Player;
using UnityEngine;

namespace Character_Movement.Controllers
{
    /// <summary>
    /// Headbob animation example:
    ///
    /// This example shows how to create a custom first person controller, this extends the BaseFirstPerson controller
    /// and adds Headbob animation. To do this, we animate the cameraPivot transform, this way we can tailor fit the camera
    /// headbob animation to match our game needs, additionally, we can use Animation events to trigger sound effects like footsteps, etc.
    /// </summary>

    public class CustomFirstPersonController : BaseFirstPersonController
    {
        #region EDITOR EXPOSED FIELDS

        [Header("Headbob")]
        public Animator cameraAnimator;
        public float cameraAnimSpeed = 1.0f;

        #endregion

        #region FIELDS

        private IPlayerInput _input;
        private int _verticalParamId;
        private int _horizontalParamId;

        #endregion

        #region METHODS

        /// <summary>
        /// Animate camera pivot to play Headbob like animations, Feed CharacterMovement info to camera animator.
        /// </summary>

        private void AnimateCamera()
        {
            var lateralVelocity = Vector3.ProjectOnPlane(movement.velocity, transform.up);
            var normalizedSpeed = Mathf.InverseLerp(0.0f, forwardSpeed, lateralVelocity.magnitude);

            cameraAnimator.speed = Mathf.Max(0.5f, cameraAnimSpeed * normalizedSpeed);

            const float dampTime = 0.1f;

            cameraAnimator.SetFloat(_verticalParamId, moveDirection.z, dampTime, Time.deltaTime);
            cameraAnimator.SetFloat(_horizontalParamId, moveDirection.x, dampTime, Time.deltaTime);
        }

        /// <summary>
        /// Override BaseFirstPersonController Animate method.
        /// </summary>

        protected override void Animate()
        {
            AnimateCamera();
        }
        
        private void HandleFootsteps()
        {
            // if (PauseController.Instance.IsPaused)
            // {
            //     return; // Do nothing if the game is paused
            // }
            
            // Check if the character is grounded and moving
            if (!isGrounded || isMoving)
            {
                AudioManager.Instance.footstepTimer = 0;
                return;
            }

            // If the footstep timer is 0 and the character is moving, play the first footstep sound immediately
            if (AudioManager.Instance.footstepTimer == 0)
            {
                PlayFootstepSound();
            }

            AudioManager.Instance.footstepTimer += Time.deltaTime;

            // Determine the current footstep delay based on whether the character is running or walking
            float currentFootstepDelay = run ? AudioManager.Instance.runningFootstepDelay : AudioManager.Instance.footstepDelay;

            if (AudioManager.Instance.footstepTimer >= currentFootstepDelay)
            {
                PlayFootstepSound();
            }
        }
        
        private void PlayFootstepSound()
        {
            if (run)
            {
                // Play running footstep sound
                AudioManager.Instance.PlayRunning(); // Replace with your actual method to play running footsteps
            }
            else
            {
                // Play walking footstep sound
                AudioManager.Instance.PlayFootstep();
            }
            
            AudioManager.Instance.footstepTimer = 0;
        }

        /// <summary>
        /// Override BaseFirstPersonController HandleInput method.
        /// </summary>

        protected override void HandleInput()
        {
            // Toggle pause / resume.
            // By default, will restore character's velocity on resume (eg: restoreVelocityOnResume = true)

            // if (Input.GetKeyDown(KeyCode.P))
            //     pause = !pause;
            
            // Use GetPrimaryAxis for movement direction
            Vector2 inputAxis = _input.GetPrimaryAxis();
            moveDirection = new Vector3
            {
                x = inputAxis.x,
                y = 0.0f,
                z = inputAxis.y
            };

            // Check if run is held
            run = _input.IsRunHeld();

            // Check if jump is held
            jump = _input.IsJumpHeld();
        }

        /// <summary>
        /// Override BaseFirstPersonController Awake method.
        /// </summary>

        public override void Awake()
        {
            // Initialize BaseFirstPersonController

            base.Awake();

            // Cache animator parameter, state ids and input component
            _input = GetComponent<IPlayerInput>();
            _verticalParamId = Animator.StringToHash("vertical");
            _horizontalParamId = Animator.StringToHash("horizontal");
        }

        public override void Update()
        {
            base.Update();
            
            HandleFootsteps();
        }

        #endregion
    }
}
