using UnityEngine;

public class RoomDefinition : MonoBehaviour
{
    public int Doors;
    public bool DoorLeft;
    public bool DoorRight;
    public bool DoorUp;
    public bool DoorDown;

    public int Enemies;

    private GameObject player;
    private int EnemiesRemaining;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        EnemiesRemaining = Enemies;
    }

    public void EnemyKilled()
    {
        EnemiesRemaining--;
        if(EnemiesRemaining <= 0)
        {
            player.GetComponent<PlayerCombat>().room_cleared = true;
        }
    }


}
