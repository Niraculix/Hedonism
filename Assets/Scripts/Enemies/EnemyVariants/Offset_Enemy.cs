using UnityEngine;

public class Enemy_Offset : MonoBehaviour
{
    public int ProjectileDamage = 5;
    public float TimerCooldown = 5f;
    public float AimOffset = 2f;

    public GameObject ProjectilePrefab;
    private Transform _playerPos;
    private float ShootTimer = 0f;

    public bool active = true;

    void Start()
    {
        _playerPos = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void FixedUpdate()
    {
        if(!transform.parent.GetComponent<Enemy>().LogicEnabled) return;

        ShootTimer -= Time.fixedDeltaTime;
        if(ShootTimer <= 0 && active)
        {
            ShootTimer = TimerCooldown;
            Shoot();
        }
    }

    void Shoot()
    {
        Vector2 playerPos = _playerPos.position;
        Vector2 toPlayer = (playerPos - (Vector2)transform.position).normalized;
        Vector2 perp = new Vector2(-toPlayer.y, toPlayer.x);
        Vector2 targetLeft = playerPos + perp * AimOffset;
        Vector2 targetRight = playerPos + perp * -AimOffset;

        FireAt(targetLeft);
        FireAt(targetRight);
    }

    void FireAt(Vector2 targetPos)
    {
        Vector2 dir = (targetPos - (Vector2)transform.position).normalized;
        GameObject proj = Instantiate(ProjectilePrefab, transform.position, Quaternion.identity);
        proj.GetComponent<Projectile>().Init(dir, ProjectileDamage);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.skyBlue;
        Gizmos.DrawSphere(transform.position,0.3f);
    }
}
