using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public Item item;
    GameObject player;
    ItemManager itemManager;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        itemManager = GameObject.FindGameObjectWithTag("ItemManager").GetComponent<ItemManager>();

        // Assign the sprite from the Item data to the SpriteRenderer
        if (item != null && item.sprite != null)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr == null)
            {
                sr = gameObject.AddComponent<SpriteRenderer>();
            }

            // Convert Texture2D to Sprite
            sr.sprite = Sprite.Create(
                item.sprite,
                new Rect(0, 0, item.sprite.width, item.sprite.height),
                new Vector2(0.5f, 0.5f)
            );
        }
        else
        {
            Debug.LogWarning("ItemObject: Item or item.sprite is null!", this);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            itemManager.ItemList.Add(item);
            itemManager.UpdateItems();
            Destroy(gameObject);
        }
    }
}