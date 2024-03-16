using System.Collections;
using System.Collections.Generic;
using HEScripts.Items;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shotgun", menuName = "Weapons/Shotgun Data")]
public class ShotgunData : ReloadableWeaponData
{
    public float spread;
}
