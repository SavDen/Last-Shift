using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class TTTT : NetworkBehaviour
{
    // private void Update()
    // {
    //     if(!IsOwner) return;
    //     
    //     if (Input.GetKeyDown(KeyCode.T))
    //     {
    //         RequestSlotServerRpc();
    //     }
    //     
    // }

    [ServerRpc]
    public void RequestSlotServerRpc()
    {
        // ⚠️ Этот код выполнится ТОЛЬКО на СЕРВЕРЕ
        // Даже если его вызвали с клиента
        Debug.Log("Выполняется на сервере!");
        
        MovePlayerTargetRpc(Owner, Vector3.back);
    }

    [TargetRpc]
    public void MovePlayerTargetRpc(NetworkConnection conn, Vector3 pos)
    {
        // ⚠️ Этот код выполнится ТОЛЬКО на КОНКРЕТНОМ КЛИЕНТЕ
        // Даже если его вызвали с сервера
        transform.position += pos;
        Debug.Log("Выполняется на клиенте: " + conn.ClientId);
    }
}
