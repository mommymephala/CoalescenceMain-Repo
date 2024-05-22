using System;
using System.Collections;
using Audio;
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

        [Header("Shooting Components")]
        [SerializeField] protected AttackType attackType;
        [SerializeField] private LayerMask layerMask;
        
        private Transform _weaponHolderTransform;
        private Transform _cameraPivotTransform;
        
        private Camera _camera;
        
        private RaycastHit _hitResult;
        
        private float _timeSinceLastShot;
        
        [Header("Visuals")]
        [SerializeField] private Transform muzzleTransform;
        [SerializeField] private Transform shellTransform;
        [SerializeField] private GameObject gunshotVFXPrefab;
        [SerializeField] private GameObject shellVFXPrefab;
        [SerializeField] private GameObject impactVFXPrefab;
        
        // Recoil variables
        private Vector3 _currentPlayerCameraRotation;
        private Vector3 _currentWeaponRotation;
        private Vector3 _weaponOriginalLocalPosition;
        private Vector3 _smoothRotation;

        private void Awake()
        {
            _controller = GetComponent<WeaponController>();
            _camera = Camera.main;
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
            var playerStateAllowed = !_controller.playerController.run && _controller.playerController.IsGrounded;

            return hasAmmo && canShootTiming && playerStateAllowed && !_controller.IsReloading;
        }

        private void Shoot()
        {
            _timeSinceLastShot = 0f;

            LayerMask defaultLayerMask = LayerMask.GetMask("EnvironmentDefault");

            for (var i = 0; i < _controller.weaponData.bulletsPerShot; i++)
            {
                Vector3 shootDirection = CalculateSpread(_camera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f)).direction);
                var ray = new Ray(muzzleTransform.position, shootDirection);

                if (gunshotVFXPrefab)
                    Instantiate(gunshotVFXPrefab, muzzleTransform.position, Quaternion.LookRotation(shootDirection));
                if (shellVFXPrefab)
                    Instantiate(shellVFXPrefab, shellTransform.position, Quaternion.LookRotation(shootDirection));

                // First check the hit against combat layer
                if (Physics.Raycast(ray, out RaycastHit hit, _controller.weaponData.maxDistance, layerMask))
                {
                    Debug.DrawRay(muzzleTransform.position, shootDirection * hit.distance, Color.green, 2f);
                    ProcessHit(hit);
                }

                // Then check for visual effects on Default layer
                if (Physics.Raycast(ray, out hit, _controller.weaponData.maxDistance, defaultLayerMask))
                {
                    if (impactVFXPrefab)
                    {
                        Instantiate(impactVFXPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                    }
                }
                else
                {
                    Debug.DrawRay(muzzleTransform.position, shootDirection * _controller.weaponData.maxDistance, Color.red, 2f);
                }
            }

            CalculateRecoil();
            ApplyKickback();
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
                    if (impact.PreDamageEffects != null)
                    {
                        foreach (AttackEffect effect in impact.PreDamageEffects)
                        {
                            effect.Apply(new AttackInfo { Damageable = damageable, ImpactPoint = hit.point, ImpactDir = -hit.normal });
                        }
                    }

                    damageable.Damage(impact.Damage, hit.point, -hit.normal);
                    
                    AudioManager.Instance.PlayEnemyWeakpoint(hit.collider.tag, hit.point);

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
            var distanceToTarget = Vector3.Distance(_camera.transform.position, GameManager.Instance.Player.transform.position);
            var spreadFactor = WeaponAiming.IsAiming ? 0.1f : 0.5f;  // Reduced base spread factor
            var maxSpreadIncrease = WeaponAiming.IsAiming ? 1.5f : 2.5f;  // Max spread factor when at max range

            var distanceFactor = Mathf.Pow(distanceToTarget / _controller.weaponData.maxDistance, 2);

            Vector3 spread = Random.insideUnitSphere * (_controller.weaponData.spread * spreadFactor * Mathf.Lerp(1f, maxSpreadIncrease, distanceFactor));

            return baseDirection + spread;
        }
        
        private void CalculateRecoil()
        {
            Vector3 recoilRotation = WeaponAiming.IsAiming  ? _controller.weaponData.recoilRotationAiming : _controller.weaponData.recoilRotationHipfire;

            var recoilMultiplier = 1f;
            if (_controller.playerController.IsMoving)
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
        
        private void ApplyKickback()
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