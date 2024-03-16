using FMODUnity;
using HorrorEngine;
using UnityEngine;

namespace HEScripts.Items
{
    [CreateAssetMenu(menuName = "Horror Engine/Items/Reloadable Weapon")]
    public class ReloadableWeaponData : WeaponData
    {
        public ItemData AmmoItem;
        public int MaxAmmo;
        public float ReloadDuration;
        public EventReference ShotSound;
        public EventReference ReloadSound;
        public EventReference NoAmmoSound;
    }
}