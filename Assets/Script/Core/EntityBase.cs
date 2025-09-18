using UnityEngine;
using FishNet.Connection;
using FishNet.Object;

public abstract class EntityBase : NetworkBehaviour, IDamage, IFlashDamageable
{
    public abstract void FlashEffect(float duration);

    public abstract void TakeDamage(float damage, TypeDamage typeDamage);

}
