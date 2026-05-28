using System;
using UnityEngine;

public class EnemyOffset : MonoBehaviour
{
    public int hp = 60;
    public int ProjectileDamage = 5;
    public float TimerCooldown = 3f;
    public float AimOffset = 2f;
    public GameObject ProjectilePrefab;
    public Transform FirePoint;
    private Transform _playerPos;
    private float ShootTimer;
    void Start()
    {
        _playerPos = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate()
    {
        ShootTimer -= Time.fixedDeltaTime;
        if (ShootTimer <= 0)
        {
            ShootTimer = TimerCooldown;
            Shoot();
        }
    }

    public void takeDamage(int damage)
    {
        hp -= damage;
        print("Damaged " + gameObject.name + ", HP Now: " + hp);

        if(hp <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        Destroy(gameObject);
    }

    void Shoot()
    {
        Vector2 playerPos = _playerPos.position;
        Vector2 toPlayer = (playerPos - (Vector2)FirePoint.position).normalized;
        Vector2 perp = new Vector2(-toPlayer.y, toPlayer.x);
        Vector2 targetLeft = playerPos + perp * AimOffset;
        Vector2 targetRight = playerPos + perp * -AimOffset;

        FireAt(targetLeft);
        FireAt(targetRight);
    }

    void FireAt(Vector2 targetPos)
    {
        Vector2 dir = (targetPos - (Vector2)FirePoint.position).normalized;
        GameObject proj = Instantiate(ProjectilePrefab, FirePoint.position, Quaternion.identity);
        proj.GetComponent<Projectile>().Init(dir, ProjectileDamage);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
