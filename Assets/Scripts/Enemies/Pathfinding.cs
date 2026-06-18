using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlatformerEnemyAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float jumpForce = 8f;
    public float walkSpeedMult = 2f;

    [Header("Detection Settings")]
    public float detectionRadius = 15f;
    public LayerMask playerLayer;
    public LayerMask Environment;

    [Header("Raycast Points")]
    public Transform groundCheck;
    public Transform wallCheck;
    public Transform edgeCheck;
    public float groundCheckRadius = 0.2f;
    public float wallCheckDistance = 1f;
    public float edgeCheckDistance = 1.5f;

    private Rigidbody2D rb;
    private float moveDirection = 1f; // 1 for right, -1 for left
    private bool isGrounded;
    private bool isChasing;
    private Transform player;
    private float flipCooldown = 0.5f;
    private float lastFlipTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Find the player in the scene (assuming the player is tagged "Player")
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        CheckGround();
        CheckForPlayer();

        if (isChasing && player != null)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    void CheckGround()
    {
        // Check if enemy is touching the ground
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, Environment);
    }

    void CheckForPlayer()
    {
        if (player == null) return;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        isChasing = distanceToPlayer <= detectionRadius;
    }

    void Patrol()
    {
        // --- WALL CHECK ---
        RaycastHit2D wallHit = Physics2D.Raycast(wallCheck.position, Vector2.right * moveDirection, wallCheckDistance, Environment);
        Debug.DrawRay(wallCheck.position, Vector2.right * moveDirection * wallCheckDistance, wallHit.collider ? Color.red : Color.green);

        if (wallHit.collider != null)
        {
            Debug.Log("WALL HIT! Flipping...");
            FlipDirection();
        }

        // --- EDGE CHECK ---
        RaycastHit2D edgeHit = Physics2D.Raycast(edgeCheck.position, Vector2.down, edgeCheckDistance, Environment);
        Debug.DrawRay(edgeCheck.position, Vector2.down * edgeCheckDistance, edgeHit.collider ? Color.green : Color.red);

        if (edgeHit.collider == null)
        {
            Debug.Log("EDGE DETECTED! Flipping...");
            FlipDirection();
        }

        // Apply movement
        rb.linearVelocity = new Vector2(moveDirection * walkSpeed, rb.linearVelocity.y);
    }

    void FlipDirection()
    {
        if (Time.time < lastFlipTime + flipCooldown) return;

        moveDirection *= -1;
        transform.localScale = new Vector3(Mathf.Sign(moveDirection), 1, 1); // Flips sprite visually
        lastFlipTime = Time.time;
        Debug.Log("Direction flipped! New direction: " + moveDirection);
    }

    void ChasePlayer()
    {
        // Determine direction to player
        float directionToPlayer = Mathf.Sign(player.position.x - transform.position.x);

        // Check if there is a wall blocking the path to the player
        RaycastHit2D wallHit = Physics2D.Raycast(wallCheck.position, Vector2.right * directionToPlayer, 3f, Environment);

        // If blocked by a wall AND grounded, JUMP!
        if (wallHit.collider != null )
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // Move towards player
        rb.linearVelocity = new Vector2(directionToPlayer * walkSpeed * walkSpeedMult, rb.linearVelocity.y); 

        if (directionToPlayer != moveDirection)
        {
            moveDirection = directionToPlayer;
            transform.localScale = new Vector3(Mathf.Sign(moveDirection), 1, 1);
        }
    }



    // Visualize Raycasts in the Unity Editor
    private void OnDrawGizmos()
    {
        if (wallCheck != null) Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.right * moveDirection * wallCheckDistance);
        if (edgeCheck != null) Gizmos.DrawLine(edgeCheck.position, edgeCheck.position + Vector3.down * edgeCheckDistance);
        if (groundCheck != null) Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}