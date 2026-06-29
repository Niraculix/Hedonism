using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Tooltip("0 = bewegt sich wie der Player, 1 = bewegt sich gar nicht (weit weg), negativ = Vordergrund-Effekt")]
    [Range(-0.5f, 1f)]
    public float parallaxFactor = 0.5f;

    private Transform player;
    private Vector3 startPos;
    private Vector3 playerStartPos;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        startPos = transform.position;
        playerStartPos = player.position;
    }

    void FixedUpdate()
    {
        Vector3 playerDelta = player.position - playerStartPos;
        Vector3 movement = playerDelta * parallaxFactor;

        transform.position = startPos + new Vector3(movement.x, movement.y, 0);
    }
}