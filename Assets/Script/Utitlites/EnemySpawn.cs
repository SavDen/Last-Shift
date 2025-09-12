using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public Transform target;
    public List<Transform> points;
    public float timeSpawn;
    public bool spawn;
    public EnemyData enemy;

    private void Awake()
    {
        StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        while(spawn)
        {
            yield return new WaitForSeconds(timeSpawn);
            var newEnemy = Instantiate(enemy.prefab, points[Random.Range(0, points.Count)].position, Quaternion.identity);
            newEnemy.Initialized(enemy, target);
        }
    }
    
}
