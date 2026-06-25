using UnityEngine;
using System.Collections.Generic;

public class RoomIconUI : MonoBehaviour
{
    [SerializeField] private GameObject doorNorth;
    [SerializeField] private GameObject doorSouth;
    [SerializeField] private GameObject doorEast;
    [SerializeField] private GameObject doorWest;
    [SerializeField] private GameObject currentRoomMarker;

    public void SetDoors(List<DoorDirection> doors)
    {
        doorNorth.SetActive(doors.Contains(DoorDirection.North));
        doorSouth.SetActive(doors.Contains(DoorDirection.South));
        doorEast.SetActive(doors.Contains(DoorDirection.East));
        doorWest.SetActive(doors.Contains(DoorDirection.West));
    }

    public void setCurrent(bool isCurrent)
    {
        currentRoomMarker.SetActive(isCurrent);
    }
}