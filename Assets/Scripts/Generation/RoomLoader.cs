using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLoader : MonoBehaviour
{
    public static RoomLoader Instance;

    private GameObject player;

    private GameObject currentRoomInstance;

    bool isTransitioning = false;

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
        if (!isTransitioning)
        {
            isTransitioning = true;

            //print("Transitioning...");
            

            // Alten Raum entladen
            var bullets = GameObject.FindGameObjectsWithTag("Bullet");
            foreach(GameObject bullet in bullets)
            {
                Destroy(bullet);
            }
            
            if(GameObject.FindGameObjectWithTag("LightSphere") != null)
            {
                Destroy(GameObject.FindGameObjectWithTag("LightSphere"));
            }

            if (currentRoomInstance != null)
                Destroy(currentRoomInstance);

            // Neuer Raum
            
            yield return new WaitForFixedUpdate();

            currentRoomInstance = Instantiate(roomPrefab);
            player = GameObject.FindGameObjectWithTag("Player");
            var def = currentRoomInstance.GetComponent<RoomDefinition>();

            if (comingFrom.HasValue)
            {
                def.enteredFromOtherRoom = true;
                def.comingFrom = (DoorDirection)comingFrom;
            }
            else
            {
                def.enteredFromOtherRoom = false;
            }

            
            var connections = GameManager.Instance.connections;

            // DEBUG:
            var allTriggers = currentRoomInstance.GetComponentsInChildren<DoorTrigger>();
            print($"Gefundene DoorTrigger: {allTriggers.Length}");
            foreach(var t in allTriggers)
            {
                print($"  DoorTrigger direction: {t.direction}");
            }
            print($"DoorSlots: {def.doors.Count}");
            foreach(var d in def.doors)
            {
                print($"  DoorSlot direction: {d.direction}");
            }
            // DEBUG END

            foreach (var doorSlot in def.doors)
            {

                // DoorTrigger im instanziierten Raum per Richtung suchen
                DoorTrigger trigger = currentRoomInstance
                    .GetComponentsInChildren<DoorTrigger>()
                    .FirstOrDefault(t => t.direction == doorSlot.direction);

                if (trigger == null)
                {
                    print($"Kein DoorTrigger gefunden für Richtung: {doorSlot.direction}");
                    continue;
                }

                var conn = connections.FirstOrDefault(c =>
                    (c.roomPrefabA == roomPrefab && c.doorFromA == doorSlot.direction) ||
                    (c.roomPrefabB == roomPrefab && c.doorFromB == doorSlot.direction)
                );

                if (conn == null) continue;

                bool isA = conn.roomPrefabA == roomPrefab;
                trigger.targetRoomPrefab = isA ? conn.roomPrefabB : conn.roomPrefabA;
                //print($"Setze target: {trigger.targetRoomPrefab.name} für Tür: {doorSlot.direction}");

            }
            
            

            //print("Transitioned");

            isTransitioning = false;

            // yield return Fader.FadeIn();

            yield return null;
        }
    }
}
