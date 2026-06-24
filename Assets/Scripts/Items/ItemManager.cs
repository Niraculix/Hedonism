using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [HideInInspector] public int max_hp;
    [HideInInspector] public float HealRate;
    [HideInInspector] public float DrainRate;
    [HideInInspector] public int maxDashesDarkMode;
    [HideInInspector] public int maxDashesLightMode;
    [HideInInspector] public int MeleeDamage;
    [HideInInspector] public float ParryRange;
    [HideInInspector] public float AttackCooldownDarkMode;

    [HideInInspector] public bool DashResetOnKill = false;
    [HideInInspector] public float parry_leech_percent = 0;
    
    public List<Item> ItemList = new List<Item>();
    // Update is called once per frame
    public void UpdateItems()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        max_hp = player.GetComponent<PlayerCombat>().max_hp;
        HealRate = player.GetComponent<PlayerCombat>().naturalHealRate;
        DrainRate = player.GetComponent<PlayerCombat>().naturalDrainRate;
        maxDashesDarkMode = 3;
        maxDashesLightMode = 1;
        MeleeDamage = player.GetComponent<PlayerCombat>().MeleeDamage;
        ParryRange = player.GetComponent<PlayerCombat>().ParryRange;
        AttackCooldownDarkMode = player.GetComponent<PlayerCombat>().AdrenalinAttackCooldown;
        

        foreach(Item item in ItemList)
        {
            switch (item.item_name)
            {
                case "testItem":
                maxDashesDarkMode++;
                break;

                case "testItem2":
                HealRate += 1;
                break;
            }
        }

        if(player != null) player.GetComponent<PlayerCombat>().ReloadItems();
    }
}
