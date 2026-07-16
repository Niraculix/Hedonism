using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public bool DungeonGenerationOn = true;
    [SerializeField] private List<GameObject> roomPrefabs_Start = new List<GameObject>();
    [SerializeField] private List<GameObject> roomPrefabs_1Door = new List<GameObject>();
    [SerializeField] private List<GameObject> roomPrefabs_2Door = new List<GameObject>();
    [SerializeField] private List<GameObject> roomPrefabs_3Door = new List<GameObject>();

    private HashSet<(int, DoorDirection)> connectedDoors = new HashSet<(int, DoorDirection)>();

    public List<RoomConnection> connections = new List<RoomConnection>();
    public RoomNode startNode;

    public static GameManager Instance;

    private int nodeIdCounter = 0;

    void Start()
    {
        if(!DungeonGenerationOn) return;
        
        GenerateDungeon();
    }

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void GenerateDungeon()
    {
        float rand = Random.value;
        GameObject StartRoom;

        if (rand < 0.2f)
        {
            StartRoom = roomPrefabs_Start[Random.value < 0.5f ? 0 : 1];
        }
        else if (rand < 0.8f)
        {
            StartRoom = roomPrefabs_Start[Random.value < 0.5f ? 2 : 3];
        }
        else
        {
            StartRoom = roomPrefabs_Start[4];
        }

        var startDef = StartRoom.GetComponent<RoomDefinition>();
        startNode = new RoomNode { prefab = StartRoom, id = nodeIdCounter++ };

        Queue<(RoomNode node, RoomDefinition def, int distance)> open = new();
        open.Enqueue((startNode, startDef, 0));

        while (open.Count > 0)
        {
            var (currentNode, currentDef, distance) = open.Dequeue();

            foreach (var door in currentDef.doors)
            {
                if (connectedDoors.Contains((currentNode.id, door.direction))) continue;

                int targetDoorCount = RandomizeDoorCount(distance);
                var nextPrefab = FindRoomWithDoorAndCount(Opposite(door.direction), targetDoorCount);
                if (nextPrefab == null) continue;

                var nextNode = new RoomNode { prefab = nextPrefab, id = nodeIdCounter++ };
                var nextDef = nextPrefab.GetComponent<RoomDefinition>();

                nextNode.mapPosition = currentNode.mapPosition + DirectionToOffset(door.direction);

                connections.Add(new RoomConnection
                {
                    nodeA = currentNode,
                    doorFromA = door.direction,
                    nodeB = nextNode,
                    doorFromB = Opposite(door.direction)
                });

                connectedDoors.Add((currentNode.id, door.direction));
                connectedDoors.Add((nextNode.id, Opposite(door.direction)));

                if (nextDef.doorCount > 1)
                    open.Enqueue((nextNode, nextDef, distance + 1));

                print($"Verbinde: {currentNode.prefab.name}[{currentNode.id}] ({door.direction}) → {nextNode.prefab.name}[{nextNode.id}]");
            }
        }

        print("Dungeon Complete!");
        RoomLoader.Instance.LoadStartRoom(startNode);
    }

    private GameObject FindRoomWithDoorAndCount(DoorDirection dir, int doorCount)
    {
        List<GameObject> roomPrefabs = doorCount switch
        {
            1 => roomPrefabs_1Door,
            2 => roomPrefabs_2Door,
            3 => roomPrefabs_3Door,
            _ => new List<GameObject>()
        };

        var exact = roomPrefabs
            .Where(p => p.GetComponent<RoomDefinition>().HasDoorIn(dir) && p.GetComponent<RoomDefinition>().doorCount == doorCount)
            .ToList();

        if (exact.Count > 0) return exact[Random.Range(0, exact.Count)];

        var fallback = roomPrefabs.Where(p => p.GetComponent<RoomDefinition>().HasDoorIn(dir)).ToList();
        return fallback.Count > 0 ? fallback[Random.Range(0, fallback.Count)] : null;
    }

    public static DoorDirection Opposite(DoorDirection dir) => dir switch
    {
        DoorDirection.North => DoorDirection.South,
        DoorDirection.South => DoorDirection.North,
        DoorDirection.East => DoorDirection.West,
        DoorDirection.West => DoorDirection.East,
        _ => dir
    };

    private Vector2Int DirectionToOffset(DoorDirection dir) => dir switch
    {
        DoorDirection.North => new Vector2Int(0, 1),
        DoorDirection.South => new Vector2Int(0, -1),
        DoorDirection.East  => new Vector2Int(1, 0),
        DoorDirection.West  => new Vector2Int(-1, 0),
        _ => Vector2Int.zero
    };

    private int RandomizeDoorCount(int distance)
    {
        float roll = Random.value;
        if (distance == 0) return roll < 0.70f ? 2 : 3;
        return 1;
    }

    public System.Collections.IEnumerator FreezeGame(float sec, float delay )
    {

        yield return new WaitForSecondsRealtime(delay);
        var time = Time.timeScale;
        Time.timeScale = 0.1f;
        print($"Freeze Game for {sec} sec");
        yield return new WaitForSecondsRealtime(sec);
        Time.timeScale = 1f;
    }
}