using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public GameObject Player;
    public Slider slider;
    public GameObject fill;

    // Update is called once per frame
    void FixedUpdate()
    {
        float hp_difference = (float)Player.GetComponent<PlayerCombat>().GetHp() / (float)Player.GetComponent<PlayerCombat>().max_hp;
        slider.value = hp_difference;
        if(Player.GetComponent<PlayerCombat>().light_dropped)
        {
            fill.GetComponent<Image>().color = Color.red;
        }
        else
        {
            fill.GetComponent<Image>().color = Color.white;
        }
    }
}
