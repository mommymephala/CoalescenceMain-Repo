using System;
using UnityEngine;

namespace Weapon
{
    public class WeaponAiming : MonoBehaviour
    {
        private WeaponController _controller;

        [SerializeField] private Transform adsTransform;
        [SerializeField] private float aimDownSightSpeed = 5f;
        [SerializeField] private float aimFOV = 40f;
        [SerializeField] private bool toggleAim = true;

        private float _originalFOV;
        private Vector3 _originalAdsPosition;
        
        public static bool IsAiming { get; private set; }

        private void Awake()
        {
            _controller = GetComponent<WeaponController>();
            
            if (_controller != null)
            {
                _controller.OnStartReload += ExitAimDownSight;
            }
            
            _originalFOV = _controller.playerCamera.fieldOfView;
            _originalAdsPosition = adsTransform.localPosition;
        }

        private void Update()
        {
            ProcessAimingInput();
            UpdateAiming();
        }
        
        private void ProcessAimingInput()
        {
            if (_controller.playerController.isGrounded && !_controller.playerController.run)
            {
                if (toggleAim && _controller.Input.IsAimingDown())
                {
                    IsAiming = !IsAiming;
                }
                else if (!toggleAim)
                {
                    IsAiming = _controller.Input.IsAimingHeld();
                }
            }
            else
            {
                IsAiming = false;
            }
        }

        private void UpdateAiming()
        {
            if (IsAiming)
            {
                AimDownSights();
            }
            else
            {
                ResetAiming();
            }
        }

        private void AimDownSights()
        {
            var targetScreenPosition = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            var distanceFromCamera = Vector3.Distance(adsTransform.position, _controller.playerCamera.transform.position);
            Vector3 targetWorldPosition = _controller.playerCamera.ScreenToWorldPoint(new Vector3(targetScreenPosition.x, targetScreenPosition.y, distanceFromCamera));
            Vector3 targetLocalPosition = adsTransform.parent.InverseTransformPoint(targetWorldPosition);

            adsTransform.localPosition = Vector3.Lerp(adsTransform.localPosition, targetLocalPosition, Time.deltaTime * aimDownSightSpeed);
            AdjustFOV(aimFOV);
            _controller.mouseLook.SetAimingDownSight(true);
        }

        private void ResetAiming()
        {
            adsTransform.localPosition = Vector3.Lerp(adsTransform.localPosition, _originalAdsPosition, Time.deltaTime * aimDownSightSpeed);
            AdjustFOV(_originalFOV);
            _controller.mouseLook.SetAimingDownSight(false);
        }
        
        private void AdjustFOV(float targetFOV)
        {
            _controller.playerCamera.fieldOfView = Mathf.Lerp(_controller.playerCamera.fieldOfView, targetFOV, Time.deltaTime * aimDownSightSpeed);
            _controller.weaponCamera.fieldOfView = Mathf.Lerp(_controller.weaponCamera.fieldOfView, targetFOV, Time.deltaTime * aimDownSightSpeed);
        }

        private void ExitAimDownSight()
        {
            IsAiming = false;
            ResetAiming();
        }

        private void OnDisable()
        {
            if (_controller != null)
            {
                _controller.OnStartReload -= ExitAimDownSight;
            }
        }
    }
}