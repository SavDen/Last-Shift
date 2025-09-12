using UnityEngine;


public enum WeaponHands
{
    OneHand,
    TwoHand
}

public class ShotableData: WeaponData
{
    public WeaponHands weaponHands;
    public float Ammo;
    public float AmmoCapacity;
    //public float ReloadTimeWeapon;
}
