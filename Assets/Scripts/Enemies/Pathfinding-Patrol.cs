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
    public Transform detectionCheck;
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
    private float moveDirection = 1f; 
    private bool isGrounded;
    private bool isChasing;
    private Transform player;
    private float flipCooldown = 0.5f;
    private float lastFlipTime = 0f;

    [Header("Optimization")]
    public float detectionRate = 0.2f; 
    private float detectionTimer;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Find the player in the scene (assuming the player is tagged "Player")
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void FixedUpdate()
    {
        if(!GetComponent<Enemy>().LogicEnabled) return;

        if (isChasing && player != null)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }

        detectionTimer -= Time.deltaTime;
        if (detectionTimer <= 0f)
        {
            CheckGround();
            CheckForPlayer();
            detectionTimer = detectionRate; 
        }
    }

    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, Environment);
    }

    void CheckForPlayer()
    {
        if (player == null) return;

      
        Vector2 checkPosition = detectionCheck != null ? detectionCheck.position : transform.position;
        float distanceToPlayer = Vector2.Distance(checkPosition, player.position);

        if (distanceToPlayer <= detectionRadius)
        {
         
            Vector2 enemyCenter = transform.position;
            Vector2 directionToPlayer = ((Vector2)player.position - enemyCenter).normalized;
            float distanceFromEnemy = Vector2.Distance(enemyCenter, player.position);

          
            int combinedLayerMask = playerLayer | Environment;

         
            RaycastHit2D hit = Physics2D.Raycast(enemyCenter, directionToPlayer, distanceFromEnemy, combinedLayerMask);

           
            

            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                isChasing = true; 
            }
            else
            {
                isChasing = false; 
            }
        }
        else
        {
            isChasing = false; 
        }
    }

    void Patrol()
    {
        // --- WALL CHECK ---
        RaycastHit2D wallHit = Physics2D.Raycast(wallCheck.position, Vector2.right * moveDirection, wallCheckDistance, Environment);
        Debug.DrawRay(wallCheck.position, Vector2.right * moveDirection * wallCheckDistance, wallHit.collider ? Color.red : Color.green);

        if (wallHit.collider != null)
        {
            FlipDirection();
        }

        // --- EDGE CHECK ---
        RaycastHit2D edgeHit = Physics2D.Raycast(edgeCheck.position, Vector2.down, edgeCheckDistance, Environment);
        Debug.DrawRay(edgeCheck.position, Vector2.down * edgeCheckDistance, edgeHit.collider ? Color.green : Color.red);

        if (edgeHit.collider == null)
        {
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




    private void OnDrawGizmos()
    {
        if (wallCheck != null) Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.right * moveDirection * wallCheckDistance);
        if (edgeCheck != null) Gizmos.DrawLine(edgeCheck.position, edgeCheck.position + Vector3.down * edgeCheckDistance);
        if (groundCheck != null) Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        // Calculate the position for the detection radius
        Vector2 checkPosition = detectionCheck != null ? detectionCheck.position : transform.position;

        // Draw a semi-transparent red circle
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawSphere(checkPosition, detectionRadius);

        // Draw the red outline
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(checkPosition, detectionRadius);

        // Draw a yellow line from the enemy's body to the detection center
        if (detectionCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, checkPosition);
        }

        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(checkPosition, player.position);

            if (distanceToPlayer <= detectionRadius)
            {
                Vector2 directionToPlayer = ((Vector2)player.position - checkPosition).normalized;
                int combinedLayerMask = playerLayer | Environment;

                RaycastHit2D hit = Physics2D.Raycast(checkPosition, directionToPlayer, distanceToPlayer, combinedLayerMask);

               
                Gizmos.color = (hit.collider != null && hit.collider.CompareTag("Player")) ? Color.green : Color.red;

                
                Vector2 endPoint = hit.collider != null ? hit.point : player.position;
                Gizmos.DrawLine(checkPosition, endPoint);
            }
        }
    }

}