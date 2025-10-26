using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SimpleMenuUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Button createRoomButton;
    public Button joinRoomButton;
    public TMP_InputField roomCodeInput;
    public TextMeshProUGUI statusText;

    [Header("References")]
    public SimpleRoomManager roomManager;
    public RemoteConnector remoteConnector;

    void OnEnable()
    {
        createRoomButton.onClick.AddListener(CreateRoom);
        joinRoomButton.onClick.AddListener(JoinRoom);

        EventBusRooms.OnRoomCreated += OnRoomCreated;
        EventBusRooms.OnRoomJoined += OnRoomJoined;
    }

    void OnDisable()
    {
        EventBusRooms.OnRoomCreated -= OnRoomCreated;
        EventBusRooms.OnRoomJoined -= OnRoomJoined;
    }

    // Создание комнаты
    private void CreateRoom()
    {
        //if(remoteConnector.Connect())
        //{
        //    statusText.text = "Создание комнаты...";
        //    //roomManager.CreateRoom();
        //}

    }

    // Подключение к комнате
    private void JoinRoom()
    {
        string roomCode = roomCodeInput.text.Trim();

        if (string.IsNullOrEmpty(roomCode))
        {
            statusText.text = "Введите код комнаты!";
            return;
        }

        statusText.text = "Подключение к комнате...";
        roomManager.JoinRoom(roomCode);
    }

    // Обработчики событий
    private void OnRoomCreated(string roomCode)
    {
        statusText.text = $"Комната создана! Код: {roomCode}";
        Debug.Log($"Комната создана с кодом: {roomCode}");
    }

    private void OnRoomJoined(string roomCode)
    {
        statusText.text = $"Подключен к комнате {roomCode}";
        Debug.Log($"Подключен к комнате: {roomCode}");
    }
}