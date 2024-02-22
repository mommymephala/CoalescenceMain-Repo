using ECM.Common;
using ECM.Controllers;
using UnityEngine;

namespace ECM.Examples
{
    public class FPS_CustomController : BaseFirstPersonController
    {
        #region EDITOR EXPOSED FIELDS

        [Header("Headbob")]
        public Animator cameraAnimator;
        public float cameraAnimSpeed = 1.0f;

        #endregion

        #region FIELDS

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

        /// <summary>
        /// Override BaseFirstPersonController HandleInput method.
        /// </summary>

        protected override void HandleInput()
        {
            base.HandleInput();
        }

        /// <summary>
        /// Override BaseFirstPersonController Awake method.
        /// </summary>

        public override void Awake()
        {
            // Initialize BaseFirstPersonController

            base.Awake();

            // Cache animator parameter and state ids

            _verticalParamId = Animator.StringToHash("vertical");
            _horizontalParamId = Animator.StringToHash("horizontal");
        }

        #endregion
    }
}
