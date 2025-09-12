using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Melee")]
public class MeleeWeaponData : WeaponData
{
    public float AttackRange;
    public float SpeedAnim;
    public TypeDamage typeDamage;
    public LayerMask layerMask;
}
