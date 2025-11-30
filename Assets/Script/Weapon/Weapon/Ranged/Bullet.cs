using FishNet;
using FishNet.Object;
using FishNet.Connection;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    private float _damage;
    private NetworkConnection _owner;

    public void InitBullet(float damage, NetworkConnection  owner)
    {
        _damage = damage;
        _owner = owner;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out IDamage damage))
        {
            if (collision.gameObject.GetComponent<PlayerController>())
            {
                return;
            }
            damage.TakeDamage(_damage, TypeDamage.Blood);
            InstanceFinder.ServerManager.Despawn(gameObject);
        }

        else
        {
            InstanceFinder.ServerManager.Despawn(gameObject);
        }
    }
}