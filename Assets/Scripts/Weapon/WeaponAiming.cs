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

        private float _originalPlayerFOV;
        private float _originalWeaponFOV;
        private bool _isAiming;
        private Vector3 _originalAdsPosition;
        private Camera _playerCamera;
        private Camera _weaponCamera;

        private void Awake()
        {
            _playerInput = GetComponentInParent<IPlayerInput>();
            _playerCamera = GameObject.Find("Camera").GetComponent<Camera>();
            _weaponCamera = GameObject.Find("WeaponCamera").GetComponent<Camera>();
            _originalPlayerFOV = _playerCamera.fieldOfView;
            _originalWeaponFOV = _weaponCamera.fieldOfView;
            _originalAdsPosition = adsTransform.localPosition;
        }

        private void Update()
        {
            if (toggleAim)
            {
                if (_playerInput.IsAimingHeld())
                {
                    _isAiming = !_isAiming;
                }
            }
            else
            {
                _isAiming = _playerInput.IsAimingHeld();
            }

            UpdateAiming();
        }

        private void UpdateAiming()
        {
            var targetFOV = _isAiming ? aimFOV : _originalPlayerFOV;
            var targetWeaponFOV = _isAiming ? aimFOV : _originalWeaponFOV;
            Vector3 targetPosition = _isAiming ? Vector3.zero : _originalAdsPosition; // Assuming that aiming position is at Vector3.zero

            _playerCamera.fieldOfView = Mathf.Lerp(_playerCamera.fieldOfView, targetFOV, Time.deltaTime * aimDownSightSpeed);
            _weaponCamera.fieldOfView = Mathf.Lerp(_weaponCamera.fieldOfView, targetWeaponFOV, Time.deltaTime * aimDownSightSpeed);
            adsTransform.localPosition = Vector3.Lerp(adsTransform.localPosition, targetPosition, Time.deltaTime * aimDownSightSpeed);
        }
    }
}