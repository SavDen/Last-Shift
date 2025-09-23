using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class StartServer : MonoBehaviour
{
    private bool runAsServer;

    void Start()
    {
#if UNITY_SERVER && !UNITY_EDITOR
        runAsServer = true; // Если билд собран как серверный
#endif

        if (runAsServer)
        {
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetConnectionData(
           "0.0.0.0", // слушаем на всех интерфейсах
           7777,      // порт
           "0.0.0.0"  // сервер-адрес для подключений (если надо Relay — здесь другая логика)
       );

            Debug.Log("Запуск сервера на 0.0.0.0:7777");
            NetworkManager.Singleton.StartServer();

        }
    }
}