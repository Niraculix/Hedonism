using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    public bool IsPaused = false;
    public bool OptionsOpened = false;

    public GameObject MenuUI;
    public GameObject OptionsUI;

    void Start()
    {
        MenuUI.SetActive(false);
    }
    
    public void Esc()
    {
        if (IsPaused)
        {
            if (!OptionsOpened)
            {
                Resume();
            }
            else
            {
                CloseOptionsMenu();
            }
        }
        else
        {
            Pause();
        }
    }

    void Pause()
    {
        MenuUI.SetActive(true);
        OptionsUI.SetActive(false);
        Time.timeScale = 0f;
        IsPaused = true;
    }

    public void Resume()
    {
        MenuUI.SetActive(false);
        Time.timeScale = 1f;
        IsPaused = false;
    }

    public void LoadOptionsMenu()
    {
        OptionsUI.SetActive(true);
        OptionsOpened = true;
    }

    public void CloseOptionsMenu()
    {
        OptionsUI.SetActive(false);
        OptionsOpened = false;
    }

    public void QuitGame()
    {
        print("Go Back to Start Screen");
    }
}
