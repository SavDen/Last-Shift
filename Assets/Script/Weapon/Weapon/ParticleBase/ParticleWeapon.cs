using System;
using FishNet.Connection;
using UnityEngine;

public class ParticleWeapon : MonoBehaviour, IShootableWeapon, IHasCooldown
{
    public Transform pointParticle;
    public Transform[] handsPos;

    private ParticleWeaponData _data;
    private bool _isNeedReload;
    private float _ammo;
    private float _ammoCapacity;
    private ParticleSystem _fire;

    ///Logic
    public float Cooldown => _data.reloadTime;
    public bool IsNeedIsNeedReload => _isNeedReload;
    public float ReloadTimeWeapon => _data.reloadTime;

    //View
    public Sprite IconWeapon => _data.icon;
    public float CurrentAmmo => _ammo;
    public float AmmoCapacity => _ammoCapacity;
    public Transform[] HandsPos => handsPos;

    public void Init(ShotableData data)
    {
        _data = data as ParticleWeaponData;
        _ammo = _data.Ammo;
        _ammoCapacity = _data.AmmoCapacity;
       _fire = Instantiate(_data.flameEffect, pointParticle.position, transform.rotation, pointParticle);
    }

    public void Shoot()
    {
        if (!_isNeedReload)
        {
            //print("Shoot");
            CheakDamage();

            _ammo -= Time.deltaTime * 5;

            if (_ammo <= 0)
            {
                //_fire.Stop();
                _isNeedReload = true;
                StopParticle();
            }
        }

        
    }

    private void CheakDamage()
    {
        var takeDamageColliders = Physics.OverlapSphere(pointParticle.position + pointParticle.forward, 1);
        foreach(var collider in takeDamageColliders)
        {
            if(collider.TryGetComponent(out IDamage damage))
            {
                if (collider.GetComponent<PlayerController>())
                {
                    return;
                }
                
                damage.TakeDamage(_data.baseDamage * Time.deltaTime, TypeDamage.Fire);
            }
        }
    }
    
    
    public void StartParticle()
    {
        _fire.Play();
    }

    public void StopParticle()
    {
        _fire.Stop();
    }

    public void ReloadWeapon()
    {
        if (_ammoCapacity <= _data.Ammo)
        {
            _ammo = _ammoCapacity;
            _ammoCapacity = 0;
        }

        else
        {
            _ammo = _data.Ammo;
            _ammoCapacity -= _data.Ammo;
        }

        _isNeedReload = false;

    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(pointParticle.position + pointParticle.forward, 1);
        Gizmos.color = Color.yellow;
    }
#endif
}
