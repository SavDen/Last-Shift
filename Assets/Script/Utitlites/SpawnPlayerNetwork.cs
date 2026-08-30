using System.Collections;
//using Unity.Cinemachine;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;
using Zenject;

public class SpawnPlayerNetwork : NetworkBehaviour 
{
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private Transform[]  _spawnPoints;
    
    [Inject] private readonly EnemyTargetManager  targetManager;

    public override void OnStartServer()
    {
        base.OnStartServer();

        StartCoroutine(SpawnAllPlayers());
    }

    private IEnumerator SpawnAllPlayers()
    {
        yield return new WaitForSeconds(2);

        int spawnIndex = 0;

        foreach (var conn in InstanceFinder.ServerManager.Clients.Values)
        {
            SpawnPlayer(conn, spawnIndex);
            spawnIndex++;
        }

    }

    private void SpawnPlayer(NetworkConnection conn, int spawnIndex)
    {
        var player = Instantiate(_playerPrefab,  _spawnPoints[spawnIndex].position, Quaternion.identity);
        targetManager.RegisterTarget(player.GetComponent<PlayerController>());
        player.GetComponent<PlayerController>().SetClass(GameSessionState.instance.GetClass(conn.ClientId));
        InstanceFinder.ServerManager.Spawn(player, conn);
    }
}
