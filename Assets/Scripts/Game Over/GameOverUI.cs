using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverUI : MonoBehaviour
{
    public GameObject text;
    public string startScreen = "Start";
    bool blinking = true;
    bool active = true;
    bool textActive;

    public AudioClip gameOverSound;

    public AudioClip UISound;

    void Start()
    {
        StartCoroutine(Blink());
        textActive = true;
        GetComponent<AudioSource>().volume = 0.5f;
        GetComponent<AudioSource>().PlayOneShot(gameOverSound);
    }

    void OnJump()
    {
        if(active)
        {
            text.SetActive(false);
            active = false;
            StartCoroutine(ChangeScene());
        }
    }

    IEnumerator ChangeScene()
    {
        blinking = false;
        text.SetActive(true);
        GetComponent<AudioSource>().PlayOneShot(UISound);
        yield return new WaitForSeconds(0.5f);
        text.SetActive(false);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Start");
    }


    IEnumerator Blink()
    {
        yield return new WaitForSeconds(0.5f);
        if(blinking)
        {
            textActive = !textActive;
            text.SetActive(textActive);
            StartCoroutine(Blink());
        }
    }
}