using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public DoorDirection direction;
    [HideInInspector] public GameObject targetRoomPrefab;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        //if (targetRoomPrefab == null) return;
        
        RoomLoader.Instance.LoadRoom(targetRoomPrefab, GameManager.Opposite(direction));
        print(targetRoomPrefab);
    }
}
