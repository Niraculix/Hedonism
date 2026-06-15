using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLoader : MonoBehaviour
{
    public static RoomLoader Instance;

    private GameObject player;

    private GameObject currentRoomInstance;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadRoom(GameObject roomPrefab, DoorDirection comingFrom)
    {
        StartCoroutine(Transition(roomPrefab, comingFrom));
    }

    public void LoadStartRoom(GameObject roomPrefab)
    {
        StartCoroutine(Transition(roomPrefab, null));
    }

    private IEnumerator Transition(GameObject roomPrefab, DoorDirection? comingFrom)
    {
        // yield return Fader.FadeOut();

        // Alten Raum entladen
        if (currentRoomInstance != null)
            Destroy(currentRoomInstance);

        // Neuer Raum
        currentRoomInstance = Instantiate(roomPrefab);
        player = GameObject.FindGameObjectWithTag("Player");

        
        var def = currentRoomInstance.GetComponent<RoomDefinition>();
        var connections = GameManager.Instance.connections;

        foreach (var doorSlot in def.doors)
        {
            if (doorSlot.doorObject == null) continue;

            doorSlot.doorObject.direction = doorSlot.direction;

            var conn = connections.FirstOrDefault(c =>
                (c.roomPrefabA == roomPrefab && c.doorFromA == doorSlot.direction) ||
                (c.roomPrefabB == roomPrefab && c.doorFromB == doorSlot.direction)
            );

            bool isA = conn.roomPrefabA == roomPrefab;
            doorSlot.doorObject.targetRoomPrefab = isA ? conn.roomPrefabB : conn.roomPrefabA;

        }
        
        if (comingFrom.HasValue)
        {
            var spawnDoor = def.doors.FirstOrDefault(d => d.direction == comingFrom.Value);
            //if (spawnDoor != null)
                //player.transform.position = spawnDoor.doorObject.transform.position;
            
            //COMING FROM IMPLEMENTIEREN
        }

        // yield return Fader.FadeIn();

        yield return null;
    }
}
