using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    [HideInInspector] public bool room_cleared = false;

    public List<DoorSlot> doors = new List<DoorSlot>();
    public int doorCount => doors.Count;
    [HideInInspector] public bool enteredFromOtherRoom;
    [HideInInspector] public DoorDirection comingFrom;
    [HideInInspector] public bool doors_open;
    


    void Start()
    {
        doors_open = true;
        Enemies = gameObject.GetComponentsInChildren<Enemy>().Count();
        player = GameObject.FindGameObjectWithTag("Player");
        playerCC = player.GetComponent<CharacterController>();
        playerRB = player.GetComponent<Rigidbody2D>();
        EnemiesRemaining = Enemies;

        if(room_cleared)
        {
            foreach(Enemy enemy in gameObject.GetComponentsInChildren<Enemy>())
            {
                Destroy(enemy);
            }
        }


        if(enteredFromOtherRoom)
        {
            Vector2 newPlayerPos = new Vector2(0,0);
            foreach(DoorTrigger door in gameObject.GetComponentsInChildren<DoorTrigger>())
            {
                print($"Tür Richtung: {door.direction}, Position: {door.transform.position}");
                if(door.direction == comingFrom)
                {
                    playerCC.doors_enterable = false;

                    playerCC.DisableMoveForSec(1);

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
                    StartCoroutine(LockDoors(1));
                }
            }

            player.transform.position = newPlayerPos;
            print($"Player Pos: {player.transform.position}");
            return;
        }
    }

    public void EnemyKilled()
    {
        EnemiesRemaining--;
        if(EnemiesRemaining <= 0)
        {
            player.GetComponent<PlayerCombat>().room_cleared = true;
            RoomLoader.Instance.CurrentRoomCleared();
            StartCoroutine(UnlockDoors(1));
        }
    }

    public bool HasDoorIn(DoorDirection dir) => doors.Any(d => d.direction == dir);

    public IEnumerator LockDoors(float sec)
    {
        yield return new WaitForSeconds(sec);
        doors_open = false;

        if(EnemiesRemaining <= 0)
        {
            StartCoroutine(UnlockDoors(1));
        }

    }
    public IEnumerator UnlockDoors(float sec)
    {
        yield return new WaitForSeconds(sec);
        doors_open = true;
    }

    void OnValidate()
    {
        if (CamBox == null) return;
        CamBox.GetComponent<BoxCollider2D>().size = new Vector2(roomSizeInCells.x * 72, roomSizeInCells.y * 40);
    }
}
