using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : RangedWeapon
{
    new public Shotgundata weaponData;

    public override void Attack() {
        base.Attack();
        // Shotgun attack logic
        Debug.Log("Shotgun-specific attack logic");
    }
}
