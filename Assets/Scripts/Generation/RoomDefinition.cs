using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class RoomDefinition : MonoBehaviour
{
    [HideInInspector] public int Enemies;

    [SerializeField] private Vector2Int roomSizeInCells = new Vector2Int(2, 1);

    public int distanceFromStart;

    [SerializeField] private GameObject CamBox;
    private GameObject player;
    private CharacterController playerCC;
    private Rigidbody2D playerRB;
    private int EnemiesRemaining;

    public List<DoorSlot> doors = new List<DoorSlot>();
    public int doorCount => doors.Count;
    [HideInInspector] public bool enteredFromOtherRoom;
    [HideInInspector] public DoorDirection comingFrom;
    //[HideInInspector] public bool doors_useable;


    void Start()
    {
        Enemies = gameObject.GetComponentsInChildren<Enemy>().Count();
        player = GameObject.FindGameObjectWithTag("Player");
        playerCC = player.GetComponent<CharacterController>();
        playerRB = player.GetComponent<Rigidbody2D>();
        EnemiesRemaining = Enemies;

        if(enteredFromOtherRoom)
        {
            Vector2 newPlayerPos = new Vector2(0,0);
            foreach(DoorTrigger door in gameObject.GetComponentsInChildren<DoorTrigger>())
            {
                print($"Tür Richtung: {door.direction}, Position: {door.transform.position}");
                if(door.direction == comingFrom)
                {
                    playerCC.doors_enterable = false;

                    //playerCC.DisableMoveForSec(1);

                    switch(comingFrom)
                    {
                        case DoorDirection.North:
                        newPlayerPos = door.SouthPoint.transform.position;
                        StartCoroutine(playerCC.LaunchPlayerInDir(new Vector2(0,-10000)));
                        break;

                        case DoorDirection.East:
                        newPlayerPos = door.WestPoint.transform.position;
                        StartCoroutine(playerCC.LaunchPlayerInDir(new Vector2(-10000,0)));
                        break;

                        case DoorDirection.South:
                        newPlayerPos = door.NorthPoint.transform.position;
                        StartCoroutine(playerCC.LaunchPlayerInDir(new Vector2(0,15000)));
                        break;

                        case DoorDirection.West:
                        newPlayerPos = door.EastPoint.transform.position;
                        StartCoroutine(playerCC.LaunchPlayerInDir(new Vector2(10000,0)));
                        break;

                    }
                }
            }

            player.transform.position = newPlayerPos;
            print($"Player Pos: {player.transform.position}");
        }
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
        CamBox.GetComponent<BoxCollider2D>().size = new Vector2(roomSizeInCells.x * 72, roomSizeInCells.y * 40);
    }
}
