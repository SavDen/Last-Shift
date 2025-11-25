using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private EnemyTargetManager  _targetManager;
    public override void InstallBindings()
    {
        Container.Bind<EnemyTargetManager>()
            .FromInstance(_targetManager) // ← На том же GameObject
            .AsSingle()
            .NonLazy();
        
        print("зависимтость готова");
    }
}
