using System;
using System.Collections;
using UnityEngine;

public class GrenadeBase : MonoBehaviour
{
    protected ThrowableWeaponData _data;
    protected ParticleSystem _explosion;
    protected LayerMask _layerMaskExploid;
    protected float _timeActivated;
    protected float _exploitedRadius;

    public virtual void Activated(ThrowableWeaponData data)
    {
        _data = data;
        _explosion = data.ParticleGranade;
        _layerMaskExploid = data.LayerMask;
        _timeActivated = data.timeActivatedExploid;
        _exploitedRadius = data.explosionRadius;
        StartCoroutine(Exploidted());
    }

    private IEnumerator Exploidted()
    {
        yield return new WaitForSeconds(_timeActivated);
        Instantiate(_explosion, transform.position, Quaternion.identity).Play();
        ApplyGrenade();
    }

    public virtual void ApplyGrenade()
    {

    }

    private IEnumerator AddForceExplision(Collider[] forceExplosion)
    {
        yield return new WaitForSeconds(0.5f);

        foreach (var damageCollider in forceExplosion)
        {
            if (damageCollider.TryGetComponent(out Rigidbody rigidbody))
            {
                rigidbody.AddExplosionForce(250, transform.position, 10, 6, ForceMode.Impulse);
            }
        }
    }
}
