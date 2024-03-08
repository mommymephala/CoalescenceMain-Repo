using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private IWeapons currentWeapon;

    public void SwitchWeapon(IWeapons newWeapon) {
        currentWeapon = newWeapon;
        newWeapon.Equip();
    }

    public void PerformAttack() {
        if (currentWeapon != null) {
            currentWeapon.Attack();

            // Execute special behaviors if applicable
          /*  if (currentWeapon is IChargeable chargeable) {
                chargeable.ChargeAttack();
            }*/
            // No extra call for IExplosive, as it's triggered within the Attack method of ThrowableWeapon
        }
    }
}
