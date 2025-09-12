using UnityEngine;

public class SmokGrenade : GrenadeBase
{
    public override void ApplyGrenade()
    {
        var smokeEffectsZone = Physics.OverlapSphere(transform.position, _exploitedRadius, _layerMaskExploid);

        if (smokeEffectsZone.Length != 0)
        {
            foreach (var smokeEffectCollider in smokeEffectsZone)
            {

                if (smokeEffectCollider.TryGetComponent(out ISmokeDamageable smokeDamageable))
                {
                    smokeDamageable.SmokeEffect();
                }
            }
        }
    }
}
