using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableWeapon  : IThrowableWeapon
{
    private TypeThrowable typeThrowable;
    private ThrowableWeaponData _data;
    private int _ammoThrowable;
    //private float lastTimeShoot;

    public float ThrowForce => _data.throwForce;

    public Sprite IconThrowable => _data.icon;
    public int AmmoThrowable => _ammoThrowable;

    public ThrowableWeapon(ThrowableWeaponData data, int countThrowable)
    {
        _data = data;
        typeThrowable = data.TypeThrowable;
        _ammoThrowable = countThrowable;
    }

    //public float Cooldown => data.coolDown;

    public void Throw(Transform grandePos, Transform minePos)
    {
        switch (typeThrowable)
        {
            case TypeThrowable.Throw:
                SpawnGranade(grandePos);
                Debug.Log("Граната");
                break;

            case TypeThrowable.Filling:
                Debug.Log("Мина");
                break;
        }

        _ammoThrowable -= 1;


        //if(UseCooldown())
        //{
        //    switch (typeThrowable)
        //    {
        //        case TypeThrowable.Throw:
        //            SpawnGranade(grandePos);
        //            Debug.Log("Граната");
        //            break;

        //        case TypeThrowable.Filling:
        //            Debug.Log("Мина");
        //            break;
        //    }
        //}

    }

    private void SpawnGranade(Transform grandePos)
    {
        GameObject grande = Object.Instantiate(_data.prefab, new Vector3(grandePos.position.x, grandePos.position.y + 2, grandePos.position.z + 0.5f), Quaternion.identity);
        Vector3 throwDirection = (grandePos.forward + Vector3.up * 0.5f).normalized; // Направление + немного вверх

        // Применяем силу не в центре, а чуть сбоку для вращения
        grande.GetComponent<Rigidbody>().AddForceAtPosition(throwDirection * _data.throwForce, grande.transform.position + Vector3.right * 0.2f, ForceMode.Impulse);
        grande.GetComponent<GrenadeBase>().Activated(_data);
    }

    //private bool UseCooldown()
    //{
    //    if (Time.time > lastTimeShoot)
    //    {
    //        lastTimeShoot = Time.time + Cooldown;
    //        return true;
    //    }

    //    else return false;
    //}
}
