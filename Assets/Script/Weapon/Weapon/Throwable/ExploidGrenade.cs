using UnityEngine;

public class ExploidGrenade: GrenadeBase
{
    private float _damage;

    public override void Activated(ThrowableWeaponData data)
    {
        _damage = (data as ThrowableExplodiedData).baseDamage;
        base.Activated(data);
    }

    public override void ApplyGrenade()
    {
        var exploidDamageCollider = Physics.OverlapSphere(transform.position, _exploitedRadius, _layerMaskExploid);

        if (exploidDamageCollider.Length != 0)
        {
            foreach (var damageCollider in exploidDamageCollider)
            {

                if (damageCollider.TryGetComponent(out IDamage damage))
                {
                    damage.TakeDamage(_damage, TypeDamage.Fire);
                }
            }
        }

    }
}
