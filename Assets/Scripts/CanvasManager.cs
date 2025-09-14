using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasManager : MonoBehaviour
{
    public void GameFieldScene()
    {
        SceneManager.LoadSceneAsync("GameField");
    }
    public void Exit()
    {
        Application.Quit();
    }
}
