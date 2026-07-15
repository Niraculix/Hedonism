using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Toggle Fullscreen;
    public Slider MusicVol;
    public TextMeshProUGUI MusicVolText;
    public Slider SFXVol;
    public TextMeshProUGUI SFXVolText;
    public Toggle StrongerVFX;

    OptionsManager optionsManager;

    void FixedUpdate()
    {
        optionsManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<OptionsManager>();
        Fullscreen.isOn = optionsManager.FullScreen;
        MusicVol.value = Mathf.Round(optionsManager.musicVolume * 20);
        SFXVol.value = Mathf.Round(optionsManager.SFXVolume * 20);
        StrongerVFX.isOn = optionsManager.StrongerVFX;

        MusicVolText.text = $"{MusicVol.value}";
        SFXVolText.text = $"{SFXVol.value}";
    }

    public void FullScreenToggled()
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<OptionsManager>().FullScreen = Fullscreen.isOn;
        print(GameObject.FindGameObjectWithTag("GameManager").GetComponent<OptionsManager>().FullScreen);
    }

    public void VFXToggled()
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<OptionsManager>().StrongerVFX = StrongerVFX.isOn;
        print(GameObject.FindGameObjectWithTag("GameManager").GetComponent<OptionsManager>().StrongerVFX);
    }

    public void MusicVolChanged()
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<OptionsManager>().musicVolume = MusicVol.value / 20;
        print(GameObject.FindGameObjectWithTag("GameManager").GetComponent<OptionsManager>().musicVolume);
    }

    public void SFXVolChanged()
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<OptionsManager>().SFXVolume = SFXVol.value / 20;
        print(GameObject.FindGameObjectWithTag("GameManager").GetComponent<OptionsManager>().SFXVolume);
    }

}
