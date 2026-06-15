using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomDefinition : MonoBehaviour
{
    public int Enemies;

    [SerializeField] private Vector2Int roomSizeInCells = new Vector2Int(2, 1);

    public int distanceFromStart;

    [SerializeField] private GameObject CamBox;

    private GameObject player;
    private int EnemiesRemaining;

    public List<DoorSlot> doors = new List<DoorSlot>();
    public int doorCount => doors.Count;

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

    public bool HasDoorIn(DoorDirection dir) => doors.Any(d => d.direction == dir);

    void OnValidate()
    {
        if (CamBox == null) return;
        CamBox.GetComponent<BoxCollider2D>().size = new Vector2(roomSizeInCells.x * 42, roomSizeInCells.y * 42);
    }


}
