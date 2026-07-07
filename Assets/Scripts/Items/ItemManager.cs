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
    
    public List<Item> PossibleItems = new List<Item>();
    public List<Item> ItemList = new List<Item>();
    // Update is called once per frame
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
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
            
            foreach(ItemAttribute att in item.attributes)
            {
                switch (att.type)
                {
                    case ItemAttribute.ItemType.MaxHp_Int:
                    max_hp += att.intValue;
                    break;

                    case ItemAttribute.ItemType.LHealRate_Float:
                    HealRate += att.floatValue;
                    break;

                    case ItemAttribute.ItemType.DDrainRate_Float:
                    DrainRate -= att.floatValue;
                    break;

                    case ItemAttribute.ItemType.DDashCount_Int:
                    maxDashesDarkMode += att.intValue;
                    break;

                    case ItemAttribute.ItemType.LDashCount_Int:
                    maxDashesLightMode += att.intValue;
                    break;

                    case ItemAttribute.ItemType.LAttackDMG_Int:
                    MeleeDamage += att.intValue;
                    break;

                    case ItemAttribute.ItemType.DAttackCooldown_Float:
                    AttackCooldownDarkMode -= att.floatValue;
                    break;

                    case ItemAttribute.ItemType.DDashReset_Bool:
                    DashResetOnKill = att.boolValue;
                    break;

                    case ItemAttribute.ItemType.LParryLechPercent_Float:
                    parry_leech_percent -= att.floatValue;
                    break;
                }

            }
        }

        if(player != null) player.GetComponent<PlayerCombat>().ReloadItems();
    }
}
