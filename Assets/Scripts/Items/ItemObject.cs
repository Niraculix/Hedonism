using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public Item item;
    GameObject player;
    ItemManager itemManager;

    RoomDefinition roomDef;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        itemManager = GameObject.FindGameObjectWithTag("ItemManager").GetComponent<ItemManager>();
        roomDef = GameObject.FindGameObjectWithTag("Room").GetComponent<RoomDefinition>();

    }

    // Diese Methode wird von ItemRoom aufgerufen NACHDEM item zugewiesen wurde
    public void InitializeItem()
    {
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