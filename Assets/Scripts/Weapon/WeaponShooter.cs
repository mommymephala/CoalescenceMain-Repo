using System;
using System.Collections;
using HEScripts.Systems;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Weapon
{
    public class WeaponShooter : MonoBehaviour
    {
        public event Action OnShootSuccess;
        
        private WeaponController _controller;

        [Header("Shooting Components")]
        [SerializeField] private Transform muzzleTransform;
        private Transform _weaponHolderTransform;
        private Transform _cameraPivotTransform;
        
        private float _timeSinceLastShot;
        
        //Recoil variables
        private Vector3 _currentPlayerCameraRotation;
        private Vector3 _currentWeaponRotation;
        private Vector3 _weaponOriginalLocalPosition;
        private Vector3 _smoothRotation;

        private void Awake()
        {
            _controller = GetComponent<WeaponController>();
            _weaponHolderTransform = GameObject.Find("WeaponHolder").transform;
            _cameraPivotTransform = GameObject.Find("Camera_Pivot").transform;
            _weaponOriginalLocalPosition = transform.localPosition;
        }

        private void Update()
        {
            if (!GameManager.Instance.IsPlaying) return;
            
            _timeSinceLastShot += Time.deltaTime;

            if (_controller.IsReloading || _controller.CurrentWeaponEntry.SecondaryCount <= 0) return;

            if (((!_controller.weaponData.allowAutoFire || !_controller.Input.IsAttackHeld()) &&
                 (_controller.weaponData.allowAutoFire || !_controller.Input.IsAttackDown())) || !CanShoot()) return;
            
            Shoot();
            OnShootSuccess?.Invoke();
        }
        
        private void LateUpdate()
        {
            ApplyRecoil();
        }

        private bool CanShoot()
        {
            return _timeSinceLastShot >= 1f / (_controller.weaponData.fireRate / 60f);
        }

        private void Shoot()
        {
            _timeSinceLastShot = 0f;
            Vector3 shootDirection = CalculateSpread(_controller.weaponCamera.transform.forward);
            
            // Implement the actual shooting logic here
            Debug.DrawRay(muzzleTransform.position, shootDirection * _controller.weaponData.maxDistance, Color.red, 2f);
            Debug.Log("Shooting.");
            
            CalculateRecoil();
            ApplyProceduralKickback();
            
            // Trigger visual and audio effects for shooting here
        }

        private Vector3 CalculateSpread(Vector3 baseDirection)
        {
            return baseDirection + Random.insideUnitSphere * _controller.weaponData.spread;
        }
        
        private void CalculateRecoil()
        {
            Vector3 recoilRotation = WeaponAiming.IsAiming  ? _controller.weaponData.recoilRotationAiming : _controller.weaponData.recoilRotationHipfire;

            var recoilMultiplier = 1f;
            if (_controller.playerController.isMoving)
            {
                recoilMultiplier = _controller.weaponData.walkingRecoilMultiplier;
            }

            _smoothRotation += new Vector3
            (
                -recoilRotation.x,
                Random.Range(-recoilRotation.y, recoilRotation.y),
                Random.Range(-recoilRotation.z, recoilRotation.z)
            ) * recoilMultiplier;
        }
        
        private void ApplyRecoil()
        {
            Vector3 currentPlayerCameraRotation = Vector3.Lerp(_currentPlayerCameraRotation, _smoothRotation, _controller.weaponData.rotationSpeed * Time.deltaTime * _controller.weaponData.smoothingFactor);
            _smoothRotation = Vector3.Lerp(_smoothRotation, Vector3.zero, _controller.weaponData.returnSpeed * Time.deltaTime * _controller.weaponData.smoothingFactor);
            _cameraPivotTransform.localRotation = Quaternion.Euler(currentPlayerCameraRotation);
            
            Vector3 currentWeaponCameraRotation = Vector3.Lerp(_currentWeaponRotation, _smoothRotation, _controller.weaponData.rotationSpeed * Time.deltaTime * _controller.weaponData.smoothingFactor);
            _smoothRotation = Vector3.Lerp(_smoothRotation, Vector3.zero, _controller.weaponData.returnSpeed * Time.deltaTime * _controller.weaponData.smoothingFactor);
            _weaponHolderTransform.localRotation = Quaternion.Euler(currentWeaponCameraRotation);

            _currentPlayerCameraRotation = currentPlayerCameraRotation;
            _currentWeaponRotation = currentWeaponCameraRotation;
        }
        
        private void ApplyProceduralKickback()
        {
            Vector3 kickbackDirection = -Vector3.forward;
            
            var kickbackAmount = Mathf.Lerp(0f, _controller.weaponData.kickbackForce, _controller.weaponData.kickbackDuration);
            Vector3 kickbackVector = kickbackDirection * kickbackAmount;
            transform.localPosition += kickbackVector;

            StartCoroutine(ResetWeaponPositionAfterKickback());
        }
        
        private IEnumerator ResetWeaponPositionAfterKickback()
        {
            yield return new WaitForSeconds(_controller.weaponData.kickbackDuration);

            var elapsedTime = 0f;
            Vector3 startPosition = transform.localPosition;
            while (elapsedTime < _controller.weaponData.resetDuration)
            {
                transform.localPosition = Vector3.Lerp(startPosition, _weaponOriginalLocalPosition, elapsedTime / _controller.weaponData.resetDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = _weaponOriginalLocalPosition;
        }
    }
}