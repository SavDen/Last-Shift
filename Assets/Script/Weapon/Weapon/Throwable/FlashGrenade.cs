using UnityEngine;

public class FlashGranade : GrenadeBase
{
    public override void ApplyGrenade()
    {
        var flashEffectsZone = Physics.OverlapSphere(transform.position, _exploitedRadius, _layerMaskExploid);

        if (flashEffectsZone.Length != 0)
        {
            foreach (var flashEffectCollider in flashEffectsZone)
            {

                if (flashEffectCollider.TryGetComponent(out IFlashDamageable flashDamageable))
                {
                    var duration = _data as ThrowableFlashData;
                    flashDamageable.FlashEffect(duration.durationFlash);
                    print("Flash");
                }
            }
        }

    }
}
