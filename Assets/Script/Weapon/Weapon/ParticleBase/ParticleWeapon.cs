using System;
using UnityEngine;

public class ParticleWeapon : MonoBehaviour, IShootableWeapon, IHasCooldown
{
    public Transform pointParticle;
    public Transform[] handsPos;

    private ParticleWeaponData _data;
    private bool reload;
    private float _ammo;
    private float _ammoCapacity;
    private ParticleSystem _fire;

    ///Logic
    public float Cooldown => _data.reloadTime;
    public bool IsNeedReload => reload;
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
        if (_ammo > 0)
        {
            //particle on
            if(_fire.isStopped)
            {

                print("start");
                _fire.Play();
            }

            CheakDamage();

            _ammo -= Time.deltaTime * 5;

            if (_ammo <= 0)
            {
                //_fire.Stop();
                reload = true;
                StopParticle();
            }
        }

        
    }

    private void CheakDamage()
    {
        var takeDamageColliders = Physics.OverlapSphere(pointParticle.position + pointParticle.forward, 1);
        foreach(var collider in takeDamageColliders)
        {
            if(collider.transform.TryGetComponent(out IDamage damage))
            {
                damage.TakeDamage(_data.baseDamage * Time.deltaTime, TypeDamage.Fire);
            }
        }
    }

    public void StopParticle()
    {
        Debug.Log("StopParticle");
        if(_fire.isPlaying)
        {
            _fire.Stop();
        }

        //particle stop
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
            _ammoCapacity -= _ammo;
        }

        reload = false;

    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(pointParticle.position + pointParticle.forward, 1);
        Gizmos.color = Color.yellow;
    }
#endif
}
