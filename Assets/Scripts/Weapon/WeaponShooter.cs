using System;
using HEScripts.Player;
using UnityEngine;

namespace Weapon
{
    public class WeaponShooter : MonoBehaviour
    {
        public event Action OnShootSuccess;
        
        private IPlayerInput _input;
        private WeaponController _controller;

        [Header("Shooting Components")]
        [SerializeField] private Transform muzzleTransform;
        private Transform _weaponHolderTransform;
        
        private float _timeSinceLastShot;

        private void Awake()
        {
            _input = GetComponentInParent<IPlayerInput>();
            _controller = GetComponent<WeaponController>();
            _weaponHolderTransform = GameObject.Find("WeaponHolder").transform;
        }

        private void Update()
        {
            _timeSinceLastShot += Time.deltaTime;

            if (_controller.IsReloading || _controller.CurrentWeaponEntry.SecondaryCount <= 0) return;

            if (((!_controller.weaponData.allowAutoFire || !_input.IsAttackHeld()) &&
                 (_controller.weaponData.allowAutoFire || !_input.IsAttackDown())) || !CanShoot()) return;
            
            Shoot();
            OnShootSuccess?.Invoke();
        }

        private bool CanShoot()
        {
            return _timeSinceLastShot >= 1f / (_controller.weaponData.fireRate / 60f);
        }

        private void Shoot()
        {
            _timeSinceLastShot = 0f;
            Vector3 shootDirection = CalculateSpread(_weaponHolderTransform.forward);
            
            // Implement the actual shooting logic here
            Debug.DrawRay(muzzleTransform.position, shootDirection * _controller.weaponData.maxDistance, Color.red, 2f);
            Debug.Log("Shooting.");
            
            // Trigger visual and audio effects for shooting here
        }

        private Vector3 CalculateSpread(Vector3 baseDirection)
        {
            return baseDirection + UnityEngine.Random.insideUnitSphere * _controller.weaponData.spread;
        }
    }
}