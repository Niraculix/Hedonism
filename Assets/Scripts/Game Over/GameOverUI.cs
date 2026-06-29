using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [Tooltip("Exakter Name deiner Hauptszene, z.B. \"GameScene\"")]
    public string mainSceneName = "GameScene";

    public void Restart()
    {
        SceneManager.LoadScene(mainSceneName);
    }
}