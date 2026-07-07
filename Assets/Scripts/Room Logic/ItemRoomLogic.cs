using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class ItemRoom : MonoBehaviour
{
    [Header("Item Spawning")]
    ItemManager itemManager;
    public GameObject itemObjectPrefab;
    public Vector2 itemSpawnPoint = Vector2.zero;

    [Header("Wave Trigger")]
    public UnityEvent onItemPickedUp;

    private bool hasWaveStarted = false;
    private GameObject spawnedItemInstance;

    public void ItemRoomStart()
    {
        itemManager = GameObject.FindGameObjectWithTag("ItemManager").GetComponent<ItemManager>();
        SpawnItem();
    }

    void FixedUpdate()
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
        if (itemManager == null) return;
        

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


        spawnedItemInstance = Instantiate(itemObjectPrefab);



        ItemObject itemObj = spawnedItemInstance.GetComponent<ItemObject>();
        if (itemObj != null)
        {
            itemObj.item = chosenItem;
            itemObj.InitializeItem(itemSpawnPoint);
            Debug.Log("ItemRoom: Item data assigned to ItemObject.");
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.aquamarine;

        Gizmos.DrawSphere(itemSpawnPoint,0.4f);
    }
}