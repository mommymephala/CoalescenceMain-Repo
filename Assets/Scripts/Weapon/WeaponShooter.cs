using System;
using System.Collections;
using Combat;
using Systems;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Weapon
{
    public class WeaponShooter : MonoBehaviour
    {
        public event Action OnShootSuccess;
        
        private WeaponController _controller;
        
        [SerializeField] private LayerMask layerMask;

        [Header("Shooting Components")]
        [SerializeField] private Transform muzzleTransform;
        [SerializeField] protected AttackType attackType;
        private Transform _weaponHolderTransform;
        private Transform _cameraPivotTransform;
        
        private RaycastHit _hitResult;

        private float _timeSinceLastShot;
        
        // Recoil variables
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

            if (!CanShoot()) return;
            if ((!_controller.weaponData.allowAutoFire && _controller.Input.IsAttackDown()) ||
                (_controller.weaponData.allowAutoFire && _controller.Input.IsAttackHeld()))
            {
                Shoot();
            }
        }
        
        private void LateUpdate()
        {
            ApplyRecoil();
        }

        private bool CanShoot()
        {
            var hasAmmo = _controller.CurrentWeaponEntry is { SecondaryCount: > 0 };
            var canShootTiming = _timeSinceLastShot >= 1f / (_controller.weaponData.fireRate / 60f);
            var playerStateAllowed = !_controller.playerController.run && _controller.playerController.isGrounded;

            return hasAmmo && canShootTiming && playerStateAllowed && !_controller.IsReloading;
        }

        private void Shoot()
        {
            _timeSinceLastShot = 0f;
            Vector3 shootDirection = CalculateSpread(_controller.weaponCamera.transform.forward);
            var ray = new Ray(muzzleTransform.position, shootDirection);

            if (Physics.Raycast(ray, out RaycastHit hit, _controller.weaponData.maxDistance, layerMask))
            {
                Debug.DrawRay(muzzleTransform.position, shootDirection * hit.distance, Color.green, 2f);
                ProcessHit(hit);
            }
            else
            {
                Debug.DrawRay(muzzleTransform.position, shootDirection * _controller.weaponData.maxDistance, Color.red, 2f);
            }

            CalculateRecoil();
            ApplyProceduralKickback();
            OnShootSuccess?.Invoke();
        }

        private void ProcessHit(RaycastHit hit)
        {
            var damageable = hit.collider.GetComponent<Damageable>();
            if (damageable)
            {
                AttackImpact impact = attackType.GetImpact(damageable.Type);
                if (impact != null)
                {
                    // Check and apply pre-damage effects if any
                    if (impact.PreDamageEffects != null)
                    {
                        foreach (AttackEffect effect in impact.PreDamageEffects)
                        {
                            effect.Apply(new AttackInfo { Damageable = damageable, ImpactPoint = hit.point, ImpactDir = -hit.normal });
                        }
                    }

                    // Apply damage
                    damageable.Damage(impact.Damage, hit.point, -hit.normal);

                    // Check and apply post-damage effects if any
                    if (impact.PostDamageEffects != null)
                    {
                        foreach (AttackEffect effect in impact.PostDamageEffects)
                        {
                            effect.Apply(new AttackInfo { Damageable = damageable, ImpactPoint = hit.point, ImpactDir = -hit.normal });
                        }
                    }
                }
            }
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