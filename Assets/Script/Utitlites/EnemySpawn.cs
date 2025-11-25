using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Object;
using UnityEngine;
using Zenject;

public class EnemySpawn : NetworkBehaviour
{
    public List<Transform> points;
    public float timeSpawn;
    public bool spawn;
    public EnemyData enemy;
    
    [Inject] private readonly EnemyTargetManager _enemyTargetManager;

    public override void OnStartServer()
    {
        base.OnStartServer();
        StartCoroutine(DelaySpawnEnemy());
    }

    private IEnumerator DelaySpawnEnemy()
    {
        yield return new WaitForSeconds(2);
        StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        while(spawn)
        {
            yield return new WaitForSeconds(timeSpawn);
            var newEnemy = Instantiate(enemy.prefab, points[Random.Range(0, points.Count)].position, Quaternion.identity);
            
            _enemyTargetManager.RegisterEnemy(newEnemy.GetComponent<EnemyBase>());
            
            InstanceFinder.ServerManager.Spawn(newEnemy.gameObject);
            
            newEnemy.Initialized(enemy);
            
        }
    }
    
}
