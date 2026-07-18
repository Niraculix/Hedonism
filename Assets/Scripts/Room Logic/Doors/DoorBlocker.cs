using UnityEngine;

public class DoorBlocker : MonoBehaviour
{
    GameObject Room;
    private DoorLockAnimation lockAnim;
    private bool lastDoorsOpen = true;
    void Start()
    {
        Room = GameObject.FindGameObjectWithTag("Room");
        lockAnim = GetComponent<DoorLockAnimation>();
    }

    void FixedUpdate()
    {
        bool doorsOpen = Room.GetComponent<RoomDefinition>().doors_open;

        if (doorsOpen != lastDoorsOpen)
        {
            lastDoorsOpen = doorsOpen;

            if (doorsOpen)
            {
                lockAnim.PlayUnlockAnimation();
            }
            else
            {
                lockAnim.PlayLockAnimation();
            }

            print(doorsOpen);
        }

        GetComponent<BoxCollider2D>().isTrigger = doorsOpen;
    }
}
