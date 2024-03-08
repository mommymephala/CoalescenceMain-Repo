using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Baton : MeleeWeapon
{
    public override void Attack() {
        base.Attack();
        // Shotgun attack logic
        Debug.Log("Shotgun-specific attack logic");
    }
}
