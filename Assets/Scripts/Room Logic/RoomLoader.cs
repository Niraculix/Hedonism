using System.Linq;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class RoomLoader : MonoBehaviour
{
    public static RoomLoader Instance;

    private GameObject player;
    private GameObject currentRoomInstance;
    [HideInInspector] public RoomNode currentNode;

    [HideInInspector] public List<int> RoomsCleared;

    bool isTransitioning = false;
    public bool IsTransitioning => isTransitioning;
    private float lastTransitionTime = -10f;
    public bool JustTransitioned => Time.time - lastTransitionTime < 0.3f;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadRoom(RoomNode node, DoorDirection comingFrom)
    {
        StartCoroutine(Transition(node, comingFrom));
    }

    public void LoadStartRoom(RoomNode node)
    {
        StartCoroutine(Transition(node, null));
    }

    public void CurrentRoomCleared()
    {
        RoomsCleared.Add(currentNode.id);
    }

    private IEnumerator Transition(RoomNode node, DoorDirection? comingFrom)
    {
        if (isTransitioning) yield break;
        isTransitioning = true;

        // alten Raum entladen

        var bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bullet in bullets) Destroy(bullet);

        var lightSphere = GameObject.FindGameObjectWithTag("LightSphere");
        if (lightSphere != null) Destroy(lightSphere);

        if (currentRoomInstance != null)
            Destroy(currentRoomInstance);

        yield return new WaitForFixedUpdate();

        //neuen Raum laden

        currentRoomInstance = Instantiate(node.prefab);
        currentNode = node;
        player = GameObject.FindGameObjectWithTag("Player");
        var def = currentRoomInstance.GetComponent<RoomDefinition>();

        print($"Betrete Raum: {node.prefab.name} [ID: {node.id}]");

        node.visited = true;
        node.discovered = true;

        foreach(int roomID in RoomsCleared)
        {
            if(roomID == currentNode.id)
            {
                def.room_cleared = true;
            }
        }

        if (comingFrom.HasValue)
        {
            def.enteredFromOtherRoom = true;
            def.comingFrom = comingFrom.Value;
        }
        else
        {
            def.enteredFromOtherRoom = false;
        }

        var connections = GameManager.Instance.connections;
        foreach (var conn in connections)
        {
            if (conn.nodeA.id == node.id) conn.nodeB.discovered = true;
            if (conn.nodeB.id == node.id) conn.nodeA.discovered = true;
        }

        MinimapUI.Instance.Refresh();

        foreach (var doorSlot in def.doors)
        {
            DoorTrigger trigger = currentRoomInstance
                .GetComponentsInChildren<DoorTrigger>()
                .FirstOrDefault(t => t.direction == doorSlot.direction);

            if (trigger == null) continue;

            var conn = connections.FirstOrDefault(c =>
                (c.nodeA.id == node.id && c.doorFromA == doorSlot.direction) ||
                (c.nodeB.id == node.id && c.doorFromB == doorSlot.direction)
            );

            if (conn == null) continue;

            bool isA = conn.nodeA.id == node.id;
            trigger.targetNode = isA ? conn.nodeB : conn.nodeA;
        }

        isTransitioning = false;
        lastTransitionTime = Time.time;
        yield return null;
    }

}