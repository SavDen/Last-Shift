using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponTypeShoot
{
    FullAuto,
    Shotgun
}

[CreateAssetMenu(menuName = "Weapons/Ranged")]
public class RangedWeaponData : ShotableData
{
    public WeaponTypeShoot typeShoot;
    public GameObject bullet;
    public float bulletSpeed;
    public float rangedBullet;


}
