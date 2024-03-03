using HorrorEngine;
using UnityEngine;

namespace HEScripts.Items
{
    [CreateAssetMenu(menuName = "Horror Engine/Items/Reloadable Weapon")]
    public class ReloadableWeaponData : WeaponData
    {
        public ItemData AmmoItem;
        public int MaxAmmo;
        public AudioClip ShotSound;
        public AudioClip ReloadSound;
        public AudioClip NoAmmoSound;
    }
}