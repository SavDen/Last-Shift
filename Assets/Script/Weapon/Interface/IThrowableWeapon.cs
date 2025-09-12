using UnityEngine;

public interface IThrowableWeapon : IWeapon
{
    public Sprite IconThrowable { get; }

    void Throw(Transform grandePos, Transform minePos);
}