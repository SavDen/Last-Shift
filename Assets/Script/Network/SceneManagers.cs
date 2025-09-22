using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneManagers
{
    public static void LoadLobby()
    {
        SceneManager.LoadScene("Lobby");
    }

    public static void LoadGame()
    {
        SceneManager.LoadScene("Game");
    }

    public static void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}