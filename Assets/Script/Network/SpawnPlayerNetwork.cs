using FishNet.Object;
using UnityEngine;

public class SpawnPlayerNetwork : NetworkBehaviour 
{
    private GameObject _playerPrefab;

    public override void OnStartServer()
    {
        // Просто загружаем префаб на сервере
        _playerPrefab = Resources.Load<GameObject>("Prefab/Player");
    }

    // 📌 Публичный метод для кнопки
    public void SpawnPlayerButton()
    {
        // Вызываем спавн когда игрок нажимает кнопку
        RequestPlayerSpawn();
    }

    [ServerRpc]
    private void RequestPlayerSpawn()
    {
        if (_playerPrefab != null)
        {
            GameObject player = Instantiate(_playerPrefab, Vector3.zero, Quaternion.identity);
            Spawn(player, base.Owner);
        }
    }
}
