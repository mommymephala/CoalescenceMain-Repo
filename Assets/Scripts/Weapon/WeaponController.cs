using System;
using System.Collections;
using Character_Movement.Components;
using Character_Movement.Controllers;
using HEScripts.UI;
using Inventory;
using Items;
using Player;
using Systems;
using UI_Codebase;
using UnityEngine;

namespace Weapon
{
    public class WeaponController : MonoBehaviour
    {
        public event Action OnStartReload;

        public WeaponData weaponData;

        [HideInInspector] public IPlayerInput Input;
        [HideInInspector] public CustomFirstPersonController playerController;
        [HideInInspector] public MouseLook mouseLook;
        [HideInInspector] public Camera playerCamera;
        [HideInInspector] public Camera weaponCamera;
        
        private Coroutine _reloadCoroutine;
        
        public bool IsReloading { get; private set; }

        public InventoryEntry CurrentWeaponEntry { get; private set; }

        private void Awake()
        {
            Input = GetComponentInParent<IPlayerInput>();
            playerController = GetComponentInParent<CustomFirstPersonController>();
            mouseLook = GetComponentInParent<MouseLook>();
            playerCamera = GameObject.Find("Camera").GetComponent<Camera>();
            weaponCamera = GameObject.Find("WeaponCamera").GetComponent<Camera>();
        }

        private void OnEnable()
        {
            GameManager.Instance.Inventory.OnEquippedWeaponChanged += UpdateCurrentWeaponEntry;
            var weaponShooter = GetComponent<WeaponShooter>();
            if (weaponShooter != null)
            {
                weaponShooter.OnShootSuccess += HandleShootSuccess;
            }
            
            UpdateCurrentWeaponEntry();
        }
        
        private void UpdateCurrentWeaponEntry()
        {
            CurrentWeaponEntry = GameManager.Instance.Inventory.GetEquippedWeapon();

            if (CurrentWeaponEntry is { Item: WeaponData reloadableWeaponData })
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
            if (GameManager.Instance.IsPlaying && Input.IsReloadDown() && !IsReloading)
            {
                StartReloadProcess(CurrentWeaponEntry);
            }
        }

        private void HandleShootSuccess()
        {
            if (CurrentWeaponEntry == null) return;
            
            if (CurrentWeaponEntry.SecondaryCount > 0)
            {
                CurrentWeaponEntry.SecondaryCount--;
                Debug.Log($"Shot fired. Remaining ammo: {CurrentWeaponEntry.SecondaryCount}");
            }
            
            if (IsReloading)
            {
                Debug.Log("Cannot shoot: Currently reloading.");
                return;
            }

            if (CurrentWeaponEntry.SecondaryCount > 0) return;
            
            if (GameManager.Instance.Inventory.Contains(weaponData.ammoItem))
            {
                Debug.Log("Out of ammo, attempting to reload...");
                StartReloadProcess(CurrentWeaponEntry);
            }
            else
            {
                Debug.Log("Cannot shoot: Out of ammo and no ammo available in inventory.");
            }
        }

        private void StartReloadProcess(InventoryEntry weaponEntry)
        {
            if (IsReloading || weaponEntry.SecondaryCount >= weaponData.maxAmmo) return;

            OnStartReload?.Invoke();
            _reloadCoroutine = StartCoroutine(ReloadSequence(weaponEntry));
        }

        private IEnumerator ReloadSequence(InventoryEntry weaponEntry)
        {
            IsReloading = true;
            UIManager.Get<UIInputListener>().AddBlockingContext(this);
            
            Inventory.Inventory inventory = GameManager.Instance.Inventory;
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

            IsReloading = false;
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
                weaponShooter.OnShootSuccess -= HandleShootSuccess;
            }
            
            IsReloading = false;
            
            if (GameManager.Instance.Inventory != null)
            {
                GameManager.Instance.Inventory.OnEquippedWeaponChanged -= UpdateCurrentWeaponEntry;
            }
        }
    }
}