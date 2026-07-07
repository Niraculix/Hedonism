using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public DoorDirection direction;
    [HideInInspector] public RoomNode targetNode;
    public GameObject SpawnPoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (targetNode == null) return;
        
        if (RoomLoader.Instance.IsTransitioning) return;
        if (RoomLoader.Instance.JustTransitioned) return;
        
        if (other.GetComponent<CharacterController>().doors_enterable == true)
        {
            RoomLoader.Instance.LoadRoom(targetNode, GameManager.Opposite(direction));
        }
    }
}
