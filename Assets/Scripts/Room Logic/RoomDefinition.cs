using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Assemblies;

public class RoomDefinition : MonoBehaviour
{
    [HideInInspector] public int Enemies;

    [SerializeField] private Vector2Int roomSizeInCells = new Vector2Int(2, 1);

    public List<EnemyWaveReference> EnemyWaves = new List<EnemyWaveReference>();

    [HideInInspector] public int distanceFromStart;

    public float KillBoxY = -100;

    public bool ItemRoom = false;

    [SerializeField] private GameObject CamBox;
    private GameObject player;
    private CharacterController playerCC;
    private Rigidbody2D playerRB;
    private int EnemiesRemainingCurrentWave;
    private int CurrentWave = 0;

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
        player = GameObject.FindGameObjectWithTag("Player");
        playerCC = player.GetComponent<CharacterController>();
        playerRB = player.GetComponent<Rigidbody2D>();

        if(room_cleared)
        {
            foreach(Enemy enemy in gameObject.GetComponentsInChildren<Enemy>())
            {
                Destroy(enemy);
            }
        }

        if(!ItemRoom && EnemyWaves.Count > 0)
        {
            StartCoroutine(NextEnemyWave());
            print("SpawnNextWave");
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
            Debug.Log($"KILLBOX TRIGGERED! Player Y: {player.transform.position.y}, KillBoxY: {KillBoxY}, SpawnPointPos: {SpawnPointPos}");

        }
    }

    public void ResetPlayerToSpawnPoint()
    {
        Debug.Log($"RESET! Player wird von {player.transform.position} zu {SpawnPointPos} gesetzt");
        player.transform.position = SpawnPointPos;
        PlayerCombat playerCom = player.GetComponent<PlayerCombat>();
        playerCom.takeDamage((int)Mathf.Round(playerCom.max_hp * 0.25f), new Vector2(0,0));
    }

    public IEnumerator NextEnemyWave()
    {
        yield return new WaitForSeconds(1);
        print("Now Spawning Wave: " + CurrentWave);
        foreach(GameObject enemyObj in EnemyWaves[CurrentWave].Wave)
        {
            enemyObj.GetComponent<Enemy>().active = true;
        }

        EnemiesRemainingCurrentWave = EnemyWaves[CurrentWave].Wave.Count;
    }

    public void EnemyKilled()
    {
        EnemiesRemainingCurrentWave--;
        if(EnemiesRemainingCurrentWave <= 0)
        {
            CurrentWave++;
            if(CurrentWave > EnemyWaves.Count)
            {
                player.GetComponent<PlayerCombat>().room_cleared = true;
                RoomLoader.Instance.CurrentRoomCleared();
                StartCoroutine(UnlockDoors(1));
            }
            else
            {
                StartCoroutine(NextEnemyWave());
            }
        }
    }

    public bool HasDoorIn(DoorDirection dir) => doors.Any(d => d.direction == dir);

    public IEnumerator LockDoors(float sec)
    {
        yield return new WaitForSeconds(sec);
        doors_open = false;
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
