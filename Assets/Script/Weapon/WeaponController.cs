
using System;
using System.Collections;
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

    public void StartShoot()
    {
        selectWeapon.StartParticle();
    }

    public void StopShoot()
    {
        selectWeapon.StopParticle();
    }

    internal void ChageGrenade()
    {
        _actualGrenade++;
        throwableSlot = throwableWeapons[_actualGrenade % throwableWeapons.Length];
        weaponControllerView.UpodateAmmoThrowable(throwableSlot.IconThrowable, throwableSlot.AmmoThrowable);
    }

    public void ChangeWeapon()
    {
        
        StopShoot();
        
        if (selectWeapon == mainSlot)
        {
            selectWeapon = additionalSlot;
        }

        else
        {
            selectWeapon = mainSlot;
        }


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

    public void MelleAttack() => _meleeWeapon.ActiveMeleeWeapon();
}
