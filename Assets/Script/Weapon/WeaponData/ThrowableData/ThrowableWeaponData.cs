using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeThrowable
{
    Throw,
    Filling
}

public class ThrowableWeaponData : ScriptableObject
{
    public string WeaponName;
    public Sprite icon;
    public GameObject prefab;
    public TypeThrowable TypeThrowable;
    public ParticleSystem ParticleGranade;
    public LayerMask LayerMask;
    public float throwForce;
    public float timeActivatedExploid;
    public float explosionRadius;

}
