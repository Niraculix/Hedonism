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
    }

    void OriggerEnter2D(Collider2D collision)
    {
        if(collision != player) return;

        itemManager.ItemList.Add(item);
        itemManager.UpdateItems();

        Destroy(gameObject);
    }
}
