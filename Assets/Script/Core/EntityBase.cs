using UnityEngine;

public abstract class EntityBase : MonoBehaviour, IDamage, IFlashDamageable
{
    public abstract void FlashEffect(float duration);

    public abstract void TakeDamage(float damage, TypeDamage typeDamage);

}
