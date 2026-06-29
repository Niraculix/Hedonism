using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomDefinition : MonoBehaviour
{
    [HideInInspector] public int Enemies;

    [SerializeField] private Vector2Int roomSizeInCells = new Vector2Int(2, 1);

    [HideInInspector] public int distanceFromStart;

    public float KillBoxY = -100;

    [SerializeField] private GameObject CamBox;
    private GameObject player;
    private CharacterController playerCC;
    private Rigidbody2D playerRB;
    private int EnemiesRemaining;

    [HideInInspector] public bool room_cleared = false;

    public List<DoorSlot> doors = new List<DoorSlot>();
    public int doorCount => doors.Count;
    [HideInInspector] public bool enteredFromOtherRoom = false;
    [HideInInspector] public DoorDirection comingFrom;
    [HideInInspector] public bool doors_open;

    private Vector2 SpawnPointPos;

    


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

            foreach(DoorTrigger door in gameObject.GetComponentsInChildren<DoorTrigger>())
            {
                if(door.direction == comingFrom)
                {
                    SpawnPointPos = door.SpawnPoint.transform.position;
                }
            }
            StartCoroutine(LockDoors(1));
            

            player.transform.position = SpawnPointPos;
            print($"Player Pos: {player.transform.position}");
        }
        else
        {
            SpawnPointPos = player.transform.position;
        }
    }

    void FixedUpdate()
    {
        if(player.transform.position.y < KillBoxY)
        {
            ResetPlayerToSpawnPoint();
        }
    }

    public void ResetPlayerToSpawnPoint()
    {
        player.transform.position = SpawnPointPos;
        PlayerCombat playerCom = player.GetComponent<PlayerCombat>();
        playerCom.takeDamage((int)Mathf.Round(playerCom.max_hp * 0.25f), new Vector2(0,0));
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

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(new Vector2(0, KillBoxY), new Vector2(300, 1));
    }
}
