using System;
using System.Collections;
using HEScripts.Inventory;
using HEScripts.Items;
using HEScripts.Player;
using HEScripts.Systems;
using HEScripts.UI;
using UnityEngine;

namespace Weapon
{
    public class WeaponController : MonoBehaviour
    {
        public event Action OnStartReload;

        public ReloadableWeaponData weaponData;
        
        private InventoryEntry _currentWeaponEntry;
        private IPlayerInput _input;
        private bool _isReloading;
        private Coroutine _reloadCoroutine;

        private void Awake()
        {
            _input = GetComponentInParent<IPlayerInput>();
        }

        private void OnEnable()
        {
            GameManager.Instance.Inventory.OnEquippedWeaponChanged += UpdateCurrentWeaponEntry;
            var weaponShooter = GetComponent<WeaponShooter>();
            if (weaponShooter != null)
            {
                weaponShooter.OnShootAttempt += HandleShootAttempt;
                weaponShooter.OnShootSuccess += HandleShootSuccess;
            }
            
            UpdateCurrentWeaponEntry();
        }
        
        private void UpdateCurrentWeaponEntry()
        {
            _currentWeaponEntry = GameManager.Instance.Inventory.GetEquippedWeapon();

            if (_currentWeaponEntry is { Item: ReloadableWeaponData reloadableWeaponData })
            {
                weaponData = reloadableWeaponData;
            }
            else
            {
                Debug.LogWarning("No equipped weapon found or the equipped item is not a reloadable weapon.");
            }
        }

        private void Update()
        {
            if (_input.IsReloadDown() && !_isReloading && _currentWeaponEntry.SecondaryCount < weaponData.maxAmmo)
            {
                StartReloadProcess(_currentWeaponEntry);
            }
        }

        private void HandleShootAttempt()
        {
            if (_currentWeaponEntry == null) return;

            if (_currentWeaponEntry.SecondaryCount <= 0 && !_isReloading)
            {
                StartReloadProcess(_currentWeaponEntry);
            }
        }

        private void HandleShootSuccess()
        {
            if (_currentWeaponEntry == null) return;

            if (_currentWeaponEntry.SecondaryCount <= 0) return;
            _currentWeaponEntry.SecondaryCount--;
            Debug.Log($"Shot fired. Remaining ammo: {_currentWeaponEntry.SecondaryCount}");
        }

        private void StartReloadProcess(InventoryEntry weaponEntry)
        {
            if (_isReloading || !gameObject.activeSelf) return;

            OnStartReload?.Invoke();
            _reloadCoroutine = StartCoroutine(ReloadSequence(weaponEntry));
        }

        private IEnumerator ReloadSequence(InventoryEntry weaponEntry)
        {
            _isReloading = true;
            UIManager.Get<UIInputListener>().AddBlockingContext(this);
            
            Inventory inventory = GameManager.Instance.Inventory;
            if (weaponData.ammoItem != null && inventory.TryGet(weaponData.ammoItem, out InventoryEntry ammoEntry))
            {
                var ammoNeeded = weaponData.maxAmmo - weaponEntry.SecondaryCount;
                var ammoAvailable = Mathf.Min(ammoEntry.Count, ammoNeeded);

                yield return new WaitForSeconds(weaponData.reloadDuration);

                if (ammoAvailable > 0)
                {
                    weaponEntry.SecondaryCount += ammoAvailable;
                    inventory.Remove(ammoEntry.Item, ammoAvailable);
                    Debug.Log($"Reload complete. Ammo loaded: {ammoAvailable}");
                }
                else
                {
                    Debug.Log("No ammo available to reload.");
                }
            }
            else
            {
                Debug.Log("No ammo available to reload.");
            }

            _isReloading = false;
            UIManager.Get<UIInputListener>().RemoveBlockingContext(this);
            _reloadCoroutine = null;
        }

        private void OnDisable()
        {
            if (_reloadCoroutine != null)
            {
                StopCoroutine(_reloadCoroutine);
                _reloadCoroutine = null;
            }
            
            var weaponShooter = GetComponent<WeaponShooter>();
            if (weaponShooter != null)
            {
                weaponShooter.OnShootAttempt -= HandleShootAttempt;
                weaponShooter.OnShootSuccess -= HandleShootSuccess;
            }
            
            _isReloading = false;
            
            if (GameManager.Instance != null && GameManager.Instance.Inventory != null)
            {
                GameManager.Instance.Inventory.OnEquippedWeaponChanged -= UpdateCurrentWeaponEntry;
            }
        }
    }
}