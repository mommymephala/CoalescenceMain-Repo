using System;
using System.Collections;
using Character_Movement.Components;
using Character_Movement.Controllers;
using FMODUnity;
using Inventory;
using Items;
using Player;
using Systems;
using UI_Codebase;
using UnityEngine;
using TMPro;

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
        [HideInInspector] public Crosshair crosshair;
        [SerializeField] private Animator weaponAnimator;
        
        private Coroutine _reloadCoroutine;
        
        public bool IsReloading { get; private set; }

        public InventoryEntry CurrentWeaponEntry { get; private set; }

        private TextMeshProUGUI ammoText;

        private void Awake()
        {
            Input = GetComponentInParent<IPlayerInput>();
            playerController = GetComponentInParent<CustomFirstPersonController>();
            mouseLook = GetComponentInParent<MouseLook>();
            playerCamera = GameObject.Find("Camera").GetComponent<Camera>();
            weaponCamera = GameObject.Find("WeaponCamera").GetComponent<Camera>();
            crosshair = GameObject.Find("Crosshair")?.GetComponent<Crosshair>();
            ammoText = GameObject.Find("AmmoText")?.GetComponent<TextMeshProUGUI>();
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
                UpdateAmmoUI();
            }
            else
            {
                Debug.LogWarning("No equipped weapon found or the equipped item is not a reloadable weapon.");
            }
        }

        private void Update()
        {
            if (GameManager.Instance.IsPlaying && Input.IsReloadDown() && !IsReloading && CurrentWeaponEntry != null)
            {
                StartReloadProcess(CurrentWeaponEntry);
            }

            // Update crosshair for aiming
            if (!crosshair.IsSizeChangeActive()) 
            {
                // Only update crosshair size if no size change is active
                if (WeaponAiming.IsAiming) 
                {
                    crosshair.SetSize(new Vector2(20, 20), 0.25f);
                } 
                
                else
                {
                    crosshair.SetSize(new Vector2(40, 40), 0.25f);
                }
            }

            // Update crosshair color based on ammo
            if (CurrentWeaponEntry is { SecondaryCount: <= 0 })
            {
                crosshair.SetColor(Color.red, 0.1f);
            }
            else
            {
                crosshair.SetColor(Color.white, 0.1f);
            }
        }

        private void HandleShootSuccess()
        {
            if (CurrentWeaponEntry == null) return;
            
            if (CurrentWeaponEntry.SecondaryCount > 0)
            {
                CurrentWeaponEntry.SecondaryCount--;
                RuntimeManager.PlayOneShot(weaponData.shotSound, transform.position);
                crosshair.MultiplySize(100f, 1f, 0.1f);
                Debug.Log($"Shot fired. Remaining ammo: {CurrentWeaponEntry.SecondaryCount}");
                UpdateAmmoUI();
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
                RuntimeManager.PlayOneShot(weaponData.noAmmoSound, transform.position);
            }
        }

        private void StartReloadProcess(InventoryEntry weaponEntry)
        {
            if (IsReloading || weaponEntry.SecondaryCount >= weaponData.maxAmmo) return;

            OnStartReload?.Invoke();
            weaponAnimator.SetTrigger("ReloadTrigger");
            RuntimeManager.PlayOneShot(weaponData.reloadSound, transform.position);
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
                    UpdateAmmoUI();
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

        private void UpdateAmmoUI()
        {
            if (ammoText != null && CurrentWeaponEntry != null)
            {
                int currentAmmo = CurrentWeaponEntry.SecondaryCount;
                int maxAmmo = weaponData.maxAmmo;
                ammoText.text = $"{currentAmmo} / {maxAmmo}";
            }
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