using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    // --- State Machine ---
    private enum EnemyState { Idle, Chase }
    private EnemyState currentState;

    [Header("Optimization")]
    public float detectionRate = 0.2f;
    private float detectionTimer;

    [Header("Detection & LOS")]
    public float detectionRadius = 5f;
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;

    [Header("Movement & Floating")]
    public float chaseSpeed = 3f;
    public float returnSpeed = 2f; // Speed at which it returns to spawn
    // vertical x float
    public float floatAmplitude = 0.3f;
    public float floatFrequency = 2f;

    // Horizontal x float
    public float floatAmplitudeX = 0.5f;
    public float floatFrequencyX = 1.5f;

    private Vector3 startPosition;
    private float floatTimer;

    private Transform player;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        currentState = EnemyState.Idle;
        startPosition = transform.position;
        floatTimer = 0f;
    }

    void Update()
    {
        if (player == null) return;

        // Always increment the float timer so the bobbing wave is perfectly continuous
        floatTimer += Time.deltaTime;

        // --- Optimization: Only check for player every 'detectionRate' seconds ---
        detectionTimer -= Time.deltaTime;
        if (detectionTimer <= 0f)
        {
            detectionTimer = detectionRate;
            CheckForPlayer();
        }

        // --- State Logic ---
        switch (currentState)
        {
            case EnemyState.Idle:
                ReturnAndFloat();
                break;
            case EnemyState.Chase:
                ChasePlayer();
                break;
        }
    }

    private void CheckForPlayer()
    {
        Collider2D playerInRange = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);

        if (playerInRange != null)
        {
            Vector2 directionToPlayer = (Vector2)player.position - (Vector2)transform.position;
            float distanceToPlayer = directionToPlayer.magnitude;

            RaycastHit2D losHit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleLayer);

            if (losHit.collider == null)
            {
                currentState = EnemyState.Chase;
            }
            else
            {
                currentState = EnemyState.Idle;
            }
        }
        else
        {
            currentState = EnemyState.Idle;
        }
    }

    private void ReturnAndFloat()
    {
        // --- X-Axis (Left/Right) ---
        float targetX = startPosition.x + Mathf.Sin(floatTimer * floatFrequencyX) * floatAmplitudeX;
        float newX = Mathf.MoveTowards(transform.position.x, targetX, returnSpeed * Time.deltaTime);

        // --- Y-Axis (Up/Down) ---
        float targetY = startPosition.y + Mathf.Sin(floatTimer * floatFrequency) * floatAmplitude;
        float newY = Mathf.MoveTowards(transform.position.y, targetY, returnSpeed * Time.deltaTime);

        
        transform.position = new Vector3(newX, newY, transform.position.z);
    }

    private void ChasePlayer()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);

        // Flip sprite to face the player
        if (player.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (player.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(startPosition, 0.2f);
        }
    }
}