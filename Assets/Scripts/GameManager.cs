using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> roomPrefabs_Start = new List<GameObject>();
    [SerializeField] private List<GameObject> roomPrefabs_1Door = new List<GameObject>();
    [SerializeField] private List<GameObject> roomPrefabs_2Door = new List<GameObject>();
    [SerializeField] private List<GameObject> roomPrefabs_3Door = new List<GameObject>();
    [SerializeField] private List<GameObject> roomPrefabs_4Door = new List<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        foreach (Transform child in transform)
        {
            print(child);
            if (child.GetComponent<RoomDefinition>())
            {
                Destroy(child);
            }
        }
        //GenerateDungeon();
    }
    void GenerateDungeon()
    {
        //Choose first Room
        float rand = Random.value;

        GameObject StartRoom;
        if(rand < 0.2) //20%
        {
            StartRoom = roomPrefabs_Start[0];
        }
        else if(rand < 0.8) //60%
        {
            StartRoom = roomPrefabs_Start[1];
        }
        else //20%
        {
            StartRoom = roomPrefabs_Start[2];
        }
        print("Start Room Doors: " + StartRoom.GetComponent<RoomDefinition>().Doors);

        Instantiate(StartRoom);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
