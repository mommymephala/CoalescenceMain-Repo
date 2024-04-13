using FMODUnity;
using UnityEngine;

namespace HEScripts.Items
{
    public class WeaponData : EquipableItemData
    {
        [Header("Shooting")]
        public bool allowAutoFire;
        public float damage;
        public float maxDistance;
        [Tooltip("In RPM")] public float fireRate;
        public float spread;
        public int bulletsPerShot;
        
        [Header("Reloading")]
        public ItemData ammoItem;
        public int maxAmmo;
        public float reloadDuration;
        
        [Header("Aim Down Sight")]
        public float originalPlayerFOV;
        public float originalWeaponFOV;
        public float aimDownSightFOV;
        public float aimDownSightSpeed;
        public float zoomSpeed;
        
        [Header("Recoil Settings")]
        public Vector3 recoilRotationHipfire;
        public Vector3 recoilRotationAiming;
        public float rotationSpeed;
        public float smoothingFactor;
        public float returnSpeed;
        public float kickbackForce;
        public float kickbackDuration;
        public float resetDuration;
        public float walkingRecoilMultiplier;
        
        [Header("Audio")]
        public EventReference shotSound;
        public EventReference reloadSound;
        public EventReference noAmmoSound;
    }
}