using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [HideInInspector] public DoorDirection direction;
    [HideInInspector] public GameObject targetRoomPrefab;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        RoomLoader.Instance.LoadRoom(targetRoomPrefab, GameManager.Opposite(direction));
        print(targetRoomPrefab);
    }
}
