using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponData : ScriptableObject
{
    public string WeaponName;
    public Sprite icon;
    public GameObject prefab;
    public float baseDamage;
    public float coolDown;
    public float reloadTime;
}
