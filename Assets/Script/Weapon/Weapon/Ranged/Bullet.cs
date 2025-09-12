using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float _damage;

    public void InitBullet(float damage)
    {
        _damage = damage;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out IDamage damage))
        {
            damage.TakeDamage(_damage, TypeDamage.Blood);
            Destroy(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }
    }
}
