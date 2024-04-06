using HEScripts.Items;
using HEScripts.Player;
using UnityEngine;

namespace Weapon
{
    public class WeaponShooter : MonoBehaviour
    {
        private IPlayerInput _input;

        [Header("Weapon Data")]
        public ReloadableWeaponData weaponData;

        [Header("Shooting Components")]
        [SerializeField] private Transform muzzleTransform;
        private Transform _weaponHolderTransform;
        private float _timeSinceLastShot;

        private void Awake()
        {
            _input = GetComponentInParent<IPlayerInput>();
            _weaponHolderTransform = GameObject.Find("WeaponHolder").transform; // Ensure this finds the correct Transform
        }

        private void Update()
        {
            // Ensure we update the time since last shot every frame
            _timeSinceLastShot += Time.deltaTime;

            switch (weaponData.allowAutoFire)
            {
                // For automatic weapons, we check if the attack button is being held
                case true when _input.IsAttackHeld() && CanShoot():
                // For non-automatic weapons, we check if the attack button was just pressed
                case false when _input.IsAttackDown() && CanShoot():
                    Shoot();
                    break;
            }
        }

        private bool CanShoot()
        {
            // Ensure the weapon fires according to its fire rate
            return _timeSinceLastShot >= 1f / (weaponData.fireRate / 60f);
        }

        private void Shoot()
        {
            Debug.Log("Shooting.");
            _timeSinceLastShot = 0f; // Reset the timer immediately after a shot

            Vector3 shootDirection = CalculateSpread(_weaponHolderTransform.forward);
            // Implement the actual shooting logic here
            Debug.DrawRay(_weaponHolderTransform.position, shootDirection * weaponData.maxDistance, Color.red, 2f);

            // TODO: Trigger visual and audio effects for shooting
        }

        private Vector3 CalculateSpread(Vector3 baseDirection)
        {
            return baseDirection + Random.insideUnitSphere * weaponData.spread;
        }
    }
}