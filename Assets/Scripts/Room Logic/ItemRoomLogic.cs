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
        SpawnItem();
    }

    void Update()
    {
        if (spawnedItemInstance != null && !hasWaveStarted)
        {
            if (spawnedItemInstance == null)
            {
                hasWaveStarted = true;
                onItemPickedUp.Invoke();
            }
        }
    }

    private void SpawnItem()
    {
        if (itemManager == null)
        {
            Debug.LogError("ItemRoom: ItemManager reference is NULL! Assign it in the Inspector.", this);
            return;
        }

        if (itemManager.PossibleItems.Count == 0)
        {
            Debug.LogError("ItemRoom: PossibleItems list is empty!", this);
            return;
        }

        if (itemObjectPrefab == null)
        {
            Debug.LogError("ItemRoom: ItemObject Prefab is NULL! Assign it in the Inspector.", this);
            return;
        }

        if (itemSpawnPoint == null)
        {
            Debug.LogError("ItemRoom: Item Spawn Point is NULL! Assign it in the Inspector.", this);
            return;
        }

        int randomIndex = Random.Range(0, itemManager.PossibleItems.Count);
        Item chosenItem = itemManager.PossibleItems[randomIndex];

        Debug.Log($"ItemRoom: Spawning item: {chosenItem.item_name}");
        Debug.Log($"ItemRoom: Item sprite is null? {chosenItem.sprite == null}");
        Debug.Log($"ItemRoom: Spawn position: {itemSpawnPoint.position}");

        spawnedItemInstance = Instantiate(itemObjectPrefab, itemSpawnPoint.position, Quaternion.identity);

        Debug.Log($"ItemRoom: Instantiate successful! Object name: {spawnedItemInstance.name}");

        spawnedItemInstance = Instantiate(itemObjectPrefab, itemSpawnPoint.position, Quaternion.identity);
        Debug.Log($"Spawned object parent: {spawnedItemInstance.transform.parent?.name ?? "NO PARENT"}");

        ItemObject itemObj = spawnedItemInstance.GetComponent<ItemObject>();
        if (itemObj != null)
        {
            itemObj.item = chosenItem;
            itemObj.InitializeItem();
            Debug.Log("ItemRoom: Item data assigned to ItemObject.");
        }
        else
        {
            Debug.LogError("ItemRoom: Spawned object has NO ItemObject script!", this);
        }
    }
}