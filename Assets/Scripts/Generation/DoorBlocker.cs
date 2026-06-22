using UnityEngine;

public class DoorBlocker : MonoBehaviour
{
    GameObject Room;
    void Start()
    {
        Room = GameObject.FindGameObjectWithTag("Room");
    }

    void FixedUpdate()
    {
        if(Room.GetComponent<RoomDefinition>().doors_open)
        {
            GetComponent<BoxCollider2D>().isTrigger = true;
            GetComponent<SpriteRenderer>().enabled = false;
        }
        else
        {
            GetComponent<BoxCollider2D>().isTrigger = false;
            GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}
