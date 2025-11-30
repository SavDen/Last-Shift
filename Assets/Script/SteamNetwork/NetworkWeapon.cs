using FishNet.Object;
using UnityEngine;
using Zenject;

public class NetworkWeapon : NetworkBehaviour
{
    [Inject] private readonly NetworkWeapon _networkWeapon;
    
    
    public void CreateBullet()
    {
        
    }
}
