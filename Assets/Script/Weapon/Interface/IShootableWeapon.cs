using FishNet.Connection;
using UnityEngine;

public interface 
    IShootableWeapon : IWeapon
{
    ///Logic
    public float Cooldown { get; }
    public bool IsNeedIsNeedReload { get; }
    public float ReloadTimeWeapon { get; }
    //View
    public Sprite IconWeapon { get; }
    public float CurrentAmmo { get; }
    public float AmmoCapacity { get; }
    public Transform[] HandsPos { get; }


    void Init(ShotableData datam);

    void Shoot();

    void StartParticle();
    
    void StopParticle();

    void ReloadWeapon();

}
