using UnityEngine;
using UnityEngine.Rendering;

public class Enemy : MonoBehaviour
{
    public int hp = 30;
    public int ProjectileDamage = 5;
    public float TimerCooldown = 5f;

    public GameObject ProjectilePrefab;
    public Transform FirePoint;

    private Transform _playerPos;

    private float ShootTimer = 0f;

    void Start()
    {
        _playerPos = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void FixedUpdate()
    {
        ShootTimer -= Time.fixedDeltaTime;
        if(ShootTimer <= 0)
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
        Vector2 dir = (_playerPos.transform.position - FirePoint.position).normalized;
        GameObject proj = Instantiate(ProjectilePrefab,FirePoint.position, Quaternion.identity);
        proj.GetComponent<Projectile>().Init(dir, ProjectileDamage);
    }
}
