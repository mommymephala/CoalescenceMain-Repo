using HEScripts.Items;
using UnityEngine;
using HEScripts.Player;

namespace Weapon
{
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] private ReloadableWeaponData weaponData;
        private IPlayerInput _input;
        private int _currentAmmo;
        private bool _isReloading;

        private void Awake()
        {
            _input = GetComponentInParent<IPlayerInput>();
            var weaponShooter = GetComponent<WeaponShooter>();
            if (weaponShooter != null)
            {
                weaponShooter.OnShootAttempt += HandleShootAttempt;
                weaponShooter.OnShootSuccess += HandleShootSuccess;
            }
            else
            {
                Debug.LogError("WeaponController: No WeaponShooter component found on the child.");
            }
        }

        private void Start()
        {
            _currentAmmo = weaponData.maxAmmo;
        }

        private void Update()
        {
            if (_input.IsReloadDown() && !_isReloading && _currentAmmo < weaponData.maxAmmo)
            {
                StartReload();
            }
        }

        private void HandleShootAttempt()
        {
            if (_currentAmmo <= 0 && !_isReloading)
            {
                Debug.Log("Out of ammo. Reloading...");
                StartReload();
            }
        }

        private void HandleShootSuccess()
        {
            if (_currentAmmo > 0)
            {
                DecreaseAmmo();
            }
        }

        private void DecreaseAmmo()
        {
            _currentAmmo--;
            Debug.Log($"Shot fired. Remaining ammo: {_currentAmmo}");
        }

        private void StartReload()
        {
            _isReloading = true;
            Debug.Log("Reloading...");
            Invoke(nameof(FinishReload), weaponData.reloadDuration); // Assuming reloadDuration is a float indicating seconds
        }

        private void FinishReload()
        {
            _currentAmmo = weaponData.maxAmmo;
            _isReloading = false;
            Debug.Log("Reload complete.");
        }

        private void OnDestroy()
        {
            var weaponShooter = GetComponent<WeaponShooter>();
            if (weaponShooter != null)
            {
                weaponShooter.OnShootAttempt -= HandleShootAttempt;
                weaponShooter.OnShootSuccess -= HandleShootSuccess;
            }
        }
    }
}