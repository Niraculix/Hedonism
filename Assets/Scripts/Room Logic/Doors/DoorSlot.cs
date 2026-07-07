using System;
using UnityEngine;

public enum DoorDirection { North, South, East, West }

[Serializable]
public class DoorSlot
{
    public DoorDirection direction;
    public bool isConnected = false;
}