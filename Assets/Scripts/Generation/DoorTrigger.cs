using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public DoorDirection direction;

    [SerializeField] public GameObject NorthPoint;
    [SerializeField] public GameObject SouthPoint;
    [SerializeField] public GameObject EastPoint;
    [SerializeField] public GameObject WestPoint;
    [HideInInspector] public RoomNode targetNode;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (targetNode == null) return;

        if (other.GetComponent<CharacterController>().doors_enterable == true)
        {
            RoomLoader.Instance.LoadRoom(targetNode, GameManager.Opposite(direction));
        }
    }
}
