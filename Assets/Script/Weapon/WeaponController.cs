
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using FishNet.Connection;
using UnityEngine;

[System.Serializable]
public class WeaponController
{
    [SerializeField] private WeaponControllerView weaponControllerView;

    private IShootableWeapon mainSlot, additionalSlot;
    private ThrowableWeapon throwableSlot;
    private MeleeWeapon _meleeWeapon;
    private TrajectoryRendering trajectoryRendering;

    private IShootableWeapon selectWeapon;

    private bool _isStopParticleWeapon = true;

    private ThrowableWeapon[] throwableWeapons;
    private int _actualGrenade;
    //private int _ammoExplos, _ammoFlash, _ammoSmok;

    public float ReloadTimeWeapon => selectWeapon.ReloadTimeWeapon;
    public float VelocityGranade => throwableSlot.ThrowForce;

    public void InitWeapon(
        IShootableWeapon main, ShotableData mainData,
        IShootableWeapon additional, ShotableData additionalData,
        MeleeWeapon meleeWeapon, MeleeWeaponData meleeWeaponData,
        ThrowableWeaponData[] throwableWeaponData,
        int explodeCount, int smokeCount, int flashCount,
        LineRenderer lineRenderer, NetworkConnection owner)
    {
        mainSlot = main;
        mainSlot.Init(mainData);

        additionalSlot = additional;
        additionalSlot.Init(additionalData);

        InitThrowable(throwableWeaponData, explodeCount, smokeCount, flashCount);

        trajectoryRendering = new TrajectoryRendering(lineRenderer, throwableSlot.ThrowForce);

        _meleeWeapon = meleeWeapon;
        _meleeWeapon.Init(meleeWeaponData);

        selectWeapon = mainSlot;


       // weaponControllerView = new WeaponControllerView();

        // weaponControllerView.UpdateWeaponView(selectWeapon.IconWeapon, throwableSlot.IconThrowable,
        //     selectWeapon.CurrentAmmo, selectWeapon.AmmoCapacity,
        //     throwableSlot.AmmoThrowable);
    }

    private void InitThrowable(ThrowableWeaponData[] data, int explodeAmmo, int flashAmmo, int smokAmmo)
    {
        throwableWeapons = new ThrowableWeapon[data.Length];

        for(int i =0; i<data.Length; i++)
        {
            //throwableWeapons[i] = new ThrowableWeapon(data[i]);

            if (data[i] is ThrowableExplodiedData)
            {
                throwableWeapons[i] = new ThrowableWeapon(data[i], explodeAmmo);
            }

            else if (data[i] is ThrowableSmokData)
            {
                throwableWeapons[i] = new ThrowableWeapon(data[i], smokAmmo);
            }

            else if (data[i] is ThrowableFlashData)
            {
                throwableWeapons[i] = new ThrowableWeapon(data[i], flashAmmo);
            }

        }

        throwableSlot = throwableWeapons[0];

    }

    //private void InitTrjectoryRender()
    //{
    //    trajectoryRendering = new TrajectoryRendering();
    //}

    public void ShowTrRender(Transform pos) => trajectoryRendering.ShowTrRender(pos);

    public void ActiveRender(bool state) => trajectoryRendering.ActiveRender(state);

    public void Shoot()
    {
        selectWeapon.Shoot();
        weaponControllerView.UpdateAmmoWeapon(selectWeapon.CurrentAmmo, selectWeapon.AmmoCapacity);
    }

    public void StartShootParticle()
    {
        Debug.Log($"StartShootParticle: selectWeapon = {(selectWeapon as MonoBehaviour)?.name ?? "null"}");
        selectWeapon.StartParticle();
        
    }

    public void StopShootParticle()
    {
        mainSlot.StopParticle();
        additionalSlot.StopParticle();
    }

    internal void ChageGrenade()
    {
        _actualGrenade++;
        throwableSlot = throwableWeapons[_actualGrenade % throwableWeapons.Length];
        weaponControllerView.UpodateAmmoThrowable(throwableSlot.IconThrowable, throwableSlot.AmmoThrowable);
    }

    public void ChangeWeapon(int indexWeapon)
    {
        Debug.Log($"ChangeWeapon called: index={indexWeapon}");
        Debug.Log($"Before: selectWeapon = {(selectWeapon as MonoBehaviour)?.name ?? "null"}");
        
        switch (indexWeapon)
        {
            case 0:
                selectWeapon = mainSlot;
                break;
            case 1:
                selectWeapon = additionalSlot;
                break;
            default:
                selectWeapon = mainSlot;
                break;
        }
        
        Debug.Log($"After: selectWeapon = {(selectWeapon as MonoBehaviour)?.name ?? "null"}");
        
        weaponControllerView.UpdateWeaponView(selectWeapon.IconWeapon, throwableSlot.IconThrowable,
            selectWeapon.CurrentAmmo, selectWeapon.AmmoCapacity,
            throwableSlot.AmmoThrowable);
    }

    internal void Reload()
    {
        selectWeapon.ReloadWeapon();
        weaponControllerView.UpdateAmmoWeapon(selectWeapon.CurrentAmmo, selectWeapon.AmmoCapacity);
    }

    public void Throwable(Transform grandePos, Transform minePos)
    {
        if(throwableSlot.AmmoThrowable > 0)
        {
            throwableSlot.Throw(grandePos, minePos);
            weaponControllerView.UpodateAmmoThrowable(throwableSlot.IconThrowable, throwableSlot.AmmoThrowable);
        }
    }

    public void MelleAttack()
    {
        _meleeWeapon.gameObject.SetActive(true);
        _meleeWeapon.ActiveMeleeWeapon();   
    }
}
