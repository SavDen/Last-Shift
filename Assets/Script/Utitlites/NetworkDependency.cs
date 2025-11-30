using UnityEngine;
using Zenject;

public class NetworkDependency : MonoInstaller
{
    [SerializeField] private EnemyTargetManager  _targetManager;
    [SerializeField] private NetworkWeapon _networkWeapon;
    public override void InstallBindings()
    {
        Container.Bind<EnemyTargetManager>()
            .FromInstance(_targetManager) 
            .AsSingle()
            .NonLazy();
        print("Target Manager Initialized");
        
        Container.Bind<NetworkWeapon>()
            .FromInstance(_networkWeapon) 
            .AsSingle()
            .NonLazy();
        print("Network Weapon Initialized");
    }
}
