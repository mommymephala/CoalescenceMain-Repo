using Character_Movement.Components;
using UnityEngine;
using HEScripts.Player;

namespace Weapon
{
    public class WeaponAiming : MonoBehaviour
    {
        private IPlayerInput _playerInput;

        [SerializeField] private Transform adsTransform;
        [SerializeField] private float aimDownSightSpeed = 5f;
        [SerializeField] private float aimFOV = 40f;
        [SerializeField] private bool toggleAim = true;

        private float _originalFOV;
        private bool _isAiming;
        private Vector3 _originalAdsPosition;
        private Camera _playerCamera;
        private Camera _weaponCamera;
        private MouseLook _mouseLook;

        private void Awake()
        {
            _playerInput = GetComponentInParent<IPlayerInput>();
            _mouseLook = GetComponentInParent<MouseLook>();
            _playerCamera = GameObject.Find("Camera").GetComponent<Camera>();
            _weaponCamera = GameObject.Find("WeaponCamera").GetComponent<Camera>();
            _originalFOV = _playerCamera.fieldOfView;
            _originalAdsPosition = adsTransform.localPosition;
        }

        private void Update()
        {
            ProcessAimingInput();
            UpdateAiming();
        }
        
        private void ProcessAimingInput()
        {
            if (toggleAim && _playerInput.IsAimingDown())
            {
                _isAiming = !_isAiming;
            }
            else if (!toggleAim)
            {
                _isAiming = _playerInput.IsAimingHeld();
            }
        }

        private void UpdateAiming()
        {
            if (_isAiming)
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
            var distanceFromCamera = Vector3.Distance(adsTransform.position, _playerCamera.transform.position);
            Vector3 targetWorldPosition = _playerCamera.ScreenToWorldPoint(new Vector3(targetScreenPosition.x, targetScreenPosition.y, distanceFromCamera));
            Vector3 targetLocalPosition = adsTransform.parent.InverseTransformPoint(targetWorldPosition);

            adsTransform.localPosition = Vector3.Lerp(adsTransform.localPosition, targetLocalPosition, Time.deltaTime * aimDownSightSpeed);
            AdjustFOV(aimFOV);
            _mouseLook.SetAimingDownSight(true);
        }

        private void ResetAiming()
        {
            adsTransform.localPosition = Vector3.Lerp(adsTransform.localPosition, _originalAdsPosition, Time.deltaTime * aimDownSightSpeed);
            AdjustFOV(_originalFOV);
            _mouseLook.SetAimingDownSight(false);
        }
        
        private void AdjustFOV(float targetFOV)
        {
            _playerCamera.fieldOfView = Mathf.Lerp(_playerCamera.fieldOfView, targetFOV, Time.deltaTime * aimDownSightSpeed);
            _weaponCamera.fieldOfView = Mathf.Lerp(_weaponCamera.fieldOfView, targetFOV, Time.deltaTime * aimDownSightSpeed);
        }
    }
}