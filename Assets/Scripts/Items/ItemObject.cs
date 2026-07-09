using TMPro;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public Item item;
    GameObject player;
    ItemManager itemManager;

    public GameObject UIAttrChangePrefab;

    public GameObject DescriptionUI;
    public GameObject ItemNameUI;

    RoomDefinition roomDef;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        itemManager = GameObject.FindGameObjectWithTag("ItemManager").GetComponent<ItemManager>();
        roomDef = GameObject.FindGameObjectWithTag("Room").GetComponent<RoomDefinition>();

    }

    // Diese Methode wird von ItemRoom aufgerufen NACHDEM item zugewiesen wurde
    public void InitializeItem(Vector2 pos)
    {
        transform.position = pos;
        if (item != null && item.sprite != null)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr == null)
            {
                sr = gameObject.AddComponent<SpriteRenderer>();
            }

            Sprite newSprite = Sprite.Create(
                item.sprite,
                new Rect(0, 0, item.sprite.width, item.sprite.height),
                new Vector2(0.5f, 0.5f)
            );

            sr.sprite = newSprite;
            Debug.Log($"ItemObject: Sprite assigned successfully!");
        }
        else
        {
            Debug.LogError($"ItemObject: Cannot initialize - item is null: {item == null}, sprite is null: {item?.sprite == null}");
        }

        ItemNameUI.GetComponent<TextMeshProUGUI>().text = item.item_name;

        float Y_Offset = 128;

        foreach(ItemAttribute att in item.attributes)
        {
            GameObject newAttrUI = Instantiate(UIAttrChangePrefab);
            newAttrUI.transform.parent = DescriptionUI.transform;
            newAttrUI.transform.localScale = new Vector2(1,1);
            newAttrUI.transform.position = new Vector2(0, Y_Offset);

            Color changeColor;

            int newValueInt;
            float newValueFloat;
            bool newValueBool;

            switch (att.type)
                {
                    case ItemAttribute.ItemType.MaxHp_Int:
                        newValueInt = itemManager.max_hp + att.intValue;
                        if(newValueInt > itemManager.max_hp)
                        {
                            changeColor = Color.green;
                        }
                        else
                        {
                            changeColor = Color.red;
                        }
                        newAttrUI.GetComponent<ItemAttrUI>().Init("Max HP", $"{itemManager.max_hp}", $"{newValueInt}", changeColor);
                    break;

                    case ItemAttribute.ItemType.LHealRate_Float:
                        newValueFloat = itemManager.HealRate + att.floatValue;
                        if(newValueFloat > itemManager.HealRate)
                        {
                            changeColor = Color.green;
                        }
                        else
                        {
                            changeColor = Color.red;
                        }
                        newAttrUI.GetComponent<ItemAttrUI>().Init("Healrate:", $"{itemManager.HealRate}% / sec", $"{newValueFloat}% / sec", changeColor);
                    break;

                    case ItemAttribute.ItemType.DDrainRate_Float:
                        newValueFloat = itemManager.DrainRate + att.floatValue;
                        if(newValueFloat < itemManager.DrainRate)
                        {
                            changeColor = Color.green;
                        }
                        else
                        {
                            changeColor = Color.red;
                        }
                        newAttrUI.GetComponent<ItemAttrUI>().Init("Drainrate:", $"{itemManager.DrainRate}% / sec", $"{newValueFloat}% / sec", changeColor);
                    
                    break;

                    case ItemAttribute.ItemType.DDashCount_Int:
                        newValueInt = itemManager.maxDashesDarkMode + att.intValue;
                        if(newValueInt > itemManager.maxDashesDarkMode)
                        {
                            changeColor = Color.green;
                        }
                        else
                        {
                            changeColor = Color.red;
                        }
                        newAttrUI.GetComponent<ItemAttrUI>().Init("Max Dashes in Adrenalin:", $"{itemManager.maxDashesDarkMode}", $"{newValueInt}", changeColor);
                    break;

                    case ItemAttribute.ItemType.LDashCount_Int:
                        newValueInt = itemManager.maxDashesLightMode + att.intValue;
                        if(newValueInt > itemManager.maxDashesLightMode)
                        {
                            changeColor = Color.green;
                        }
                        else
                        {
                            changeColor = Color.red;
                        }
                        newAttrUI.GetComponent<ItemAttrUI>().Init("Max Dashes in Normal:", $"{itemManager.maxDashesLightMode}", $"{newValueInt}", changeColor);
                    break;

                    case ItemAttribute.ItemType.LAttackDMG_Int:
                        newValueInt = itemManager.MeleeDamage + att.intValue;
                        if(newValueInt > itemManager.MeleeDamage)
                        {
                            changeColor = Color.green;
                        }
                        else
                        {
                            changeColor = Color.red;
                        }
                        newAttrUI.GetComponent<ItemAttrUI>().Init("Melee Damage:", $"{itemManager.maxDashesLightMode}", $"{newValueInt}", changeColor);
                    break;

                    case ItemAttribute.ItemType.DAttackCooldown_Float:
                        newValueFloat = itemManager.AttackCooldownDarkMode + att.floatValue;
                        if(newValueFloat < itemManager.AttackCooldownDarkMode)
                        {
                            changeColor = Color.green;
                        }
                        else
                        {
                            changeColor = Color.red;
                        }
                        newAttrUI.GetComponent<ItemAttrUI>().Init("Attack Cooldown:", $"{itemManager.AttackCooldownDarkMode} sec", $"{newValueFloat} sec", changeColor);
                    break;

                    case ItemAttribute.ItemType.DDashReset_Bool:
                        newValueBool = true;
                        if(newValueBool == itemManager.DashResetOnKill)
                        {
                            changeColor = Color.white;
                        }
                        else
                        {
                            changeColor = Color.green;
                        }
                        newAttrUI.GetComponent<ItemAttrUI>().Init("Dash Resets on Kill:", $"{itemManager.DashResetOnKill} sec", $"{newValueBool} sec", changeColor);
                    break;

                    case ItemAttribute.ItemType.LParryLechPercent_Float:
                        newValueFloat = itemManager.parry_leech_percent + att.floatValue;
                        if(newValueFloat > itemManager.parry_leech_percent)
                        {
                            changeColor = Color.green;
                        }
                        else
                        {
                            changeColor = Color.red;
                        }
                        newAttrUI.GetComponent<ItemAttrUI>().Init("Heal with Parry", $"{itemManager.parry_leech_percent}% of Projectile DMG", $"{newValueFloat}% of Projectile DMG", changeColor);
                    break;
                }
            Y_Offset -= 90;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            itemManager.ItemList.Add(item);
            itemManager.UpdateItems();
            StartCoroutine(roomDef.NextEnemyWave());

            Destroy(gameObject);
        }
    }
}