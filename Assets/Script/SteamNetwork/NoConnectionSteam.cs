using UnityEngine;
using UnityEngine.SceneManagement;

public class NoConnectionSteam : MonoBehaviour
{
    public void RestartScene()
    {
        SceneManager.LoadScene(0);
    }
}
