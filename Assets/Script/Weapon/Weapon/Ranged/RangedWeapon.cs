//using System;

using FishNet;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class RangedWeapon : MonoBehaviour, IShootableWeapon, IHasCooldown
{
    public Transform[] handsPos;

    [SerializeField] private Transform _billetPos;

    private RangedWeaponData _data;

    private NetworkConnection _owner;

    private float _ammo;
    private float _ammoCapacity;
    private bool EmptyAmmo;
    private float lastTimeShoot;


    //Logic
    public float Cooldown => _data.coolDown;
    public bool IsNeedIsNeedReload => EmptyAmmo;
    public float ReloadTimeWeapon => _data.reloadTime;
    //View
    public Sprite IconWeapon => _data.icon;
    public float CurrentAmmo => _ammo;
    public float AmmoCapacity => _ammoCapacity;
    public Transform[] HandsPos => handsPos;


    public void Init(ShotableData data)
    {
        _data = data as RangedWeaponData;
        _ammo = _data.Ammo;
        _ammoCapacity = _data.AmmoCapacity;
    }

    public void Shoot()
    {
        Debug.Log($"[KIRO] RangedWeapon.Shoot() on {this.name}, ammo={_ammo}");
        if(!EmptyAmmo && CooldownShoot())
        {
            TypeShoot();
            if (_ammo <= 0) EmptyAmmo = true;
        }
        else
        {
            Debug.Log($"[KIRO] Cannot shoot: EmptyAmmo={EmptyAmmo}");
        }
    }

    public void StartParticle()
    {
        Debug.Log($"Particle + {this.name}");
    }

    public void StopParticle()
    {
        
    }

    private void TypeShoot()
    {
        switch(_data.typeShoot)
        {
            case WeaponTypeShoot.FullAuto:
                ShootFull();
                break;

            case WeaponTypeShoot.Shotgun:
                ShootGun();
                break;
        }
    }

    private void ShootGun()
    {
        _ammo -= 3;
    }

    private void ShootFull()
    {
        _ammo -= 1;
        CreateOneBullet();
    }

    
    private void CreateOneBullet()
    {
        Debug.Log($"[KIRO] CreateOneBullet on {this.name}, position={_billetPos?.position ?? Vector3.zero}");
        
        if (_billetPos == null)
        {
            Debug.LogError($"[KIRO] _billetPos is NULL on {this.name}!");
            return;
        }
        
        var newBullet = Instantiate(_data.bullet, _billetPos.position, _billetPos.rotation);
        newBullet.transform.Rotate(new Vector3(0, Random.Range(-_data.rangedBullet, _data.rangedBullet), 0));
        InstanceFinder.ServerManager.Spawn(newBullet);
        newBullet.GetComponent<Bullet>().InitBullet(_data.baseDamage, _owner);
        newBullet.GetComponent<Rigidbody>().AddForce(newBullet.transform.forward * _data.bulletSpeed, ForceMode.Impulse);
        
        Debug.Log($"[KIRO] Bullet spawned successfully!");
    }

    private bool CooldownShoot()
    {
        if (Time.time > lastTimeShoot)
        {
            lastTimeShoot = Time.time + Cooldown;
            return true;
        }

        else return false;
    }

    public void ReloadWeapon()
    {
        if(_ammoCapacity <= _data.Ammo)
        {
            _ammo = _ammoCapacity;
            _ammoCapacity = 0;
        }

        else
        {
            _ammo = _data.Ammo;
            _ammoCapacity -= _ammo;
        }


        EmptyAmmo = false;
    }
}
