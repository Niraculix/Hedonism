using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenLogic : MonoBehaviour
{
    public GameObject text;
    bool blinking = true;
    bool active = true;
    bool textActive;

    public AudioClip StartUp;

    public AudioClip soundTrack;

    AudioSource Music;
    void Start()
    {
        active = true;
        StartCoroutine(Blink());
        Music = gameObject.AddComponent<AudioSource>();
        textActive = true;

        Music.clip = soundTrack;
        Music.loop = true;
        Music.Play();
    }
    void OnJump()
    {
        if(active)
        {
            text.SetActive(false);
            GetComponent<AudioSource>().PlayOneShot(StartUp);
            active = false;
            StartCoroutine(ChangeScene());
        }
    }

    IEnumerator ChangeScene()
    {
        blinking = false;
        text.SetActive(true);
        Music.Stop();
        yield return new WaitForSeconds(1f);
        text.SetActive(false);
        yield return new WaitForSeconds(2.2f);
        SceneManager.LoadScene("GameScene");
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
