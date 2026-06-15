using System;
using UnityEngine;

public enum DoorDirection { North, South, East, West }

[Serializable]
public class DoorSlot
{
    public DoorDirection direction;
    public DoorTrigger doorObject;
    public bool isConnected = false;
}