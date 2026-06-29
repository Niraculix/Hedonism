using UnityEngine;


public class RoomNode
{
    public GameObject prefab;

    public int id;
    public Vector2Int mapPosition;
    public bool visited = false;
    public bool discovered = false;
}
