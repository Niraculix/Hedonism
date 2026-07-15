using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenLogic : MonoBehaviour
{
    public GameObject text;
    bool blinking = true;
    bool textActive;
    void Start()
    {
        StartCoroutine(Blink());
        textActive = true;
    }
    void OnJump()
    {
        text.SetActive(false);
        StartCoroutine(ChangeScene());
        print("tod");
    }

    IEnumerator ChangeScene()
    {
        blinking = false;
        yield return new WaitForSeconds(0.75f);
        SceneManager.LoadScene("GameScene");
    }

    IEnumerator Blink()
    {
        yield return new WaitForSeconds(0.2f);
        if(blinking)
        {
            textActive = !textActive;
            text.SetActive(textActive);
            StartCoroutine(Blink());
        }
    }
}
