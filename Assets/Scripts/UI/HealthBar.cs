using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public GameObject Player;
    public Slider slider;

    // Update is called once per frame
    void FixedUpdate()
    {
        float hp_difference = (float)Player.GetComponent<PlayerCombat>().GetHp() / (float)Player.GetComponent<PlayerCombat>().max_hp;
        slider.value = hp_difference;
    }
}
