using UnityEngine;

public class RoomDefinition : MonoBehaviour
{
    public int Doors;
    public bool DoorLeft;
    public bool DoorRight;
    public bool DoorUp;
    public bool DoorDown;

    public int Enemies;

    [SerializeField] private Vector2Int roomSizeInCells = new Vector2Int(2, 1);

    [SerializeField] private GameObject CamBox;

    private GameObject player;
    private int EnemiesRemaining;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        EnemiesRemaining = Enemies;
    }

    public void EnemyKilled()
    {
        EnemiesRemaining--;
        if(EnemiesRemaining <= 0)
        {
            player.GetComponent<PlayerCombat>().room_cleared = true;
        }
    }

    void OnValidate()
    {
        CamBox.GetComponent<BoxCollider2D>().size = new Vector2(roomSizeInCells.x * 72, roomSizeInCells.y * 40);
    }


}
