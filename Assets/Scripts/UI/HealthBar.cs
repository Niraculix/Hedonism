using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [HideInInspector] public GameObject Player;
    public Slider slider;
    public GameObject fill;
    public GameObject HpText;

    ItemManager itemManager;

    void Start()
    {
        itemManager = GameObject.FindGameObjectWithTag("ItemManager").GetComponent<ItemManager>();
    }
    void FixedUpdate()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        if(Player == null) return;

        float current_hp = (float)Player.GetComponent<PlayerCombat>().GetHp();
        TextMeshProUGUI textMesh = HpText.GetComponent<TextMeshProUGUI>();

        textMesh.text = $"{Mathf.Round(current_hp * 100) / 100.0} / {itemManager.max_hp}";
        
        float hp_difference =  current_hp / itemManager.max_hp;
        slider.value = hp_difference;
        if(Player.GetComponent<PlayerCombat>().light_dropped)
        {
            fill.GetComponent<Image>().color = Color.red;
            textMesh.color = Color.white;
        }
        else
        {
            fill.GetComponent<Image>().color = Color.white;
            textMesh.color = Color.black;
        }
    }
}
