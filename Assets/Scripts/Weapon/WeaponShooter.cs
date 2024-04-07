using HEScripts.Items;
using UnityEngine;
using HEScripts.Player;

namespace Weapon
{
    public class WeaponShooter : MonoBehaviour
    {
        public event System.Action OnShootAttempt;
        public event System.Action OnShootSuccess;

        [Header("Weapon Data")]
        [SerializeField] private ReloadableWeaponData weaponData;

        [Header("Shooting Components")]
        [SerializeField] private Transform muzzleTransform;
        private Transform _weaponHolderTransform;
        private float _timeSinceLastShot;
        private IPlayerInput _input;

        private void Awake()
        {
            _input = GetComponentInParent<IPlayerInput>();
            _weaponHolderTransform = GameObject.Find("WeaponHolder").transform;
        }

        private void Update()
        {
            _timeSinceLastShot += Time.deltaTime;

            if ((weaponData.allowAutoFire && _input.IsAttackHeld() || !weaponData.allowAutoFire && _input.IsAttackDown()) && CanShoot())
            {
                OnShootAttempt?.Invoke(); // Notify of the attempt to shoot
                
                Shoot();
                OnShootSuccess?.Invoke(); // Notify of successful shot
            }
        }

        private bool CanShoot()
        {
            return _timeSinceLastShot >= 1f / (weaponData.fireRate / 60f);
        }

        private void Shoot()
        {
            _timeSinceLastShot = 0f;
            Vector3 shootDirection = CalculateSpread(_weaponHolderTransform.forward);
            
            // Implement the actual shooting logic here
            Debug.DrawRay(muzzleTransform.position, shootDirection * weaponData.maxDistance, Color.red, 2f);
            
            // Trigger visual and audio effects for shooting here
        }

        private Vector3 CalculateSpread(Vector3 baseDirection)
        {
            return baseDirection + Random.insideUnitSphere * weaponData.spread;
        }
    }
}