using System;
using System.Collections;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    [SerializeField] private Transform pointDamage;

    private MeleeWeaponData _meleeWeaponData;
    private Coroutine _meleeAttackCorutine;
    private bool _completeDamage;

    public void Init(MeleeWeaponData meleeWeaponData)
    {
        _meleeWeaponData = meleeWeaponData;
    }

    public void ActiveMeleeWeapon()
    {
        _completeDamage = false;
        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(0.6f);

        while(!_completeDamage)
        {
            CheckDamage();
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void CheckDamage()
    {
        var colliderDamage = Physics.OverlapSphere(pointDamage.position, _meleeWeaponData.AttackRange, _meleeWeaponData.layerMask);

        if(colliderDamage.Length > 0)
        {
            for(int i =0; i<colliderDamage.Length; i++)
            {
                if(colliderDamage[i].TryGetComponent(out IDamage damage))
                {
                    damage.TakeDamage(_meleeWeaponData.baseDamage, _meleeWeaponData.typeDamage);
                }
            }
            _completeDamage = true;
        }

    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.lightGreen;
        Gizmos.DrawSphere(pointDamage.position, _meleeWeaponData.AttackRange);
    }
#endif 
}
