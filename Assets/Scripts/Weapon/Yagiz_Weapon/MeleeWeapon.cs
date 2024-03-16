using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour, IWeapons
{
    // public RangedWeaponData weaponData;
    
    public virtual void Attack()
    {
        // Generic ranged weapon attack logic
    }

    public virtual void Equip()
    {
        // Equip logic for ranged weapons
    }
}
