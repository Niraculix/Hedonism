using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> roomPrefabs_Start = new List<GameObject>();
    [SerializeField] private List<GameObject> roomPrefabs_1Door = new List<GameObject>();
    [SerializeField] private List<GameObject> roomPrefabs_2Door = new List<GameObject>();
    [SerializeField] private List<GameObject> roomPrefabs_3Door = new List<GameObject>();

    
    private HashSet<(GameObject, DoorDirection)> connectedDoors = new HashSet<(GameObject, DoorDirection)>();
    
    public List<RoomConnection> connections = new List<RoomConnection>();
    
    public static GameManager Instance;


    void Start()
    {
        GenerateDungeon();
    }


    private void Awake()
    {
        Instance = this;
    }
    void GenerateDungeon()
    {
        //Choose first Room
        float rand = Random.value;

        GameObject StartRoom;
        if(rand < 0.2) //20%
        {
            float rand2 = Random.value;
            if(rand2 < 0.5)
            {
                StartRoom = roomPrefabs_Start[0];
            }
            else
            {
                StartRoom = roomPrefabs_Start[1];
            }
        }
        else if(rand < 0.8) //60%
        {
            float rand2 = Random.value;
            if(rand2 < 0.5)
            {
                StartRoom = roomPrefabs_Start[2];
            }
            else
            {
                StartRoom = roomPrefabs_Start[3];
            }
        }
        else //20%
        {
            StartRoom = roomPrefabs_Start[4];
        }

        //Instantiate(StartRoom);

        Queue<(GameObject prefab, RoomDefinition def, int distance)> open = new();

        var startDef = StartRoom.GetComponent<RoomDefinition>();
        open.Enqueue((StartRoom, startDef, 0));



        while (open.Count > 0)
        {
            var (currentPrefab, currentDef, distance) = open.Dequeue();

            print("Distance from Start: " + distance);

            foreach (var door in currentDef.doors)
            {
                // Prüfen ob diese Tür dieses Prefabs schon verbunden wurde
                if (connectedDoors.Contains((currentPrefab, door.direction))) continue;

                int targetDoorCount = RandomizeDoorCount(distance);

                var nextPrefab = FindRoomWithDoorAndCount(
                    Opposite(door.direction),
                    targetDoorCount
                );

                //print("next Prefab: " + nextPrefab);
                if (nextPrefab == null) continue;

                var nextDef = nextPrefab.GetComponent<RoomDefinition>();

                connections.Add(new RoomConnection
                {
                    roomPrefabA = currentPrefab,
                    doorFromA = door.direction,
                    roomPrefabB = nextPrefab,
                    doorFromB = Opposite(door.direction)
                });

                // Beide Seiten als verbunden markieren
                connectedDoors.Add((currentPrefab, door.direction));
                connectedDoors.Add((nextPrefab, Opposite(door.direction)));

                if (nextDef.doorCount > 1)
                {
                    open.Enqueue((nextPrefab, nextDef, distance + 1));
                }
                //print($"Verbinde: {currentPrefab.name} ({door.direction}) → {nextPrefab.name}");
            }
        }

        print("Dungeon Complete!");
        RoomLoader.Instance.LoadStartRoom(StartRoom);
    }


    private GameObject FindRoomWithDoorAndCount(DoorDirection dir, int doorCount)
    {
        List<GameObject> roomPrefabs = new List<GameObject>();
        if(doorCount == 1)
        {
            roomPrefabs = roomPrefabs_1Door;
        }
        else if(doorCount == 2)
        {
            roomPrefabs = roomPrefabs_2Door;
        }
        else if(doorCount == 3)
        {
            roomPrefabs = roomPrefabs_3Door;
        }
        var exact = roomPrefabs
            .Where(p => {
                var def = p.GetComponent<RoomDefinition>();
                return def.HasDoorIn(dir) && def.doorCount == doorCount;
            }).ToList();

        if (exact.Count > 0)
            return exact[Random.Range(0, exact.Count)];

        var fallback = roomPrefabs
            .Where(p => p.GetComponent<RoomDefinition>().HasDoorIn(dir))
            .ToList();

        if (fallback.Count > 0)
            return fallback[Random.Range(0, fallback.Count)];

        return null;
    }

    public static DoorDirection Opposite(DoorDirection dir) => dir switch
    {
        DoorDirection.North => DoorDirection.South,
        DoorDirection.South => DoorDirection.North,
        DoorDirection.East  => DoorDirection.West,
        DoorDirection.West  => DoorDirection.East,
        _ => dir
    };

    private int RandomizeDoorCount(int distance)
    {
        float roll = Random.value;

        if (distance == 0)
        {
            if (roll < 0.70f) return 2;
            return 3;
        }
        else
        {
            return 1;
        }
    }
}
