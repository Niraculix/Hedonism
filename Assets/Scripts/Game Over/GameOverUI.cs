using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public string mainSceneName = "GameScene";
    public string startScreen = "Start";

    public void Restart()
    {
        SceneManager.LoadScene(mainSceneName);
    }

    public void StartScreen()
    {
        SceneManager.LoadScene(startScreen);
    }
}