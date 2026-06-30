using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class ItemRoom : MonoBehaviour
{
    [Header("Item Spawning")]
    public ItemManager itemManager;
    public GameObject itemObjectPrefab;
    public Transform itemSpawnPoint;

    [Header("Wave Trigger")]
    public UnityEvent onItemPickedUp;

    private bool hasWaveStarted = false;
    private GameObject spawnedItemInstance;

    void Start()
    {
        // Spawn the item immediately when the room starts
        SpawnItem();

    }

    void Update()
    {
        // Check if the item has been picked up (destroyed)
        if (spawnedItemInstance != null && !hasWaveStarted)
        {
            // If the spawned item reference becomes null, it means it was destroyed (picked up)
            // We check this by seeing if the GameObject is null
            if (spawnedItemInstance == null)
            {
                hasWaveStarted = true;
                Debug.Log("ItemRoom: Item picked up! Starting wave...");
                onItemPickedUp.Invoke();
            }
        }
    }

    private void SpawnItem()
    {
        if (itemManager == null || itemManager.PossibleItems.Count == 0)
        {
            Debug.LogError("ItemRoom: ItemManager is null or PossibleItems is empty!", this);
            return;
        }

        if (itemObjectPrefab == null || itemSpawnPoint == null)
        {
            Debug.LogError("ItemRoom: Prefab or SpawnPoint is null!", this);
            return;
        }

        // Pick random item from the ItemManager's PossibleItems list
        int randomIndex = Random.Range(0, itemManager.PossibleItems.Count);
        Item chosenItem = itemManager.PossibleItems[randomIndex];
        Debug.Log($"ItemRoom: Spawning item: {chosenItem.item_name}");
        Debug.Log($"ItemRoom: Item sprite is null? {chosenItem.sprite == null}");


        // Spawn the ItemObject prefab
        spawnedItemInstance = Instantiate(itemObjectPrefab, itemSpawnPoint.position, Quaternion.identity);

        // Assign the chosen item data to the spawned object
        ItemObject itemObj = spawnedItemInstance.GetComponent<ItemObject>();
        if (itemObj != null)
        {
            itemObj.item = chosenItem;
        }
        else
        {
            Debug.LogError("ItemRoom: Spawned object doesn't have ItemObject script!", this);
        }
    }
}