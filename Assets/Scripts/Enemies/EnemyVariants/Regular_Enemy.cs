using UnityEngine;

public class Projectile_Spawner_Regular : MonoBehaviour
{
    public int ProjectileDamage = 5;
    public float TimerCooldown = 5f;
    

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

    public void Shoot()
    {
        Vector2 dir = (_playerPos.transform.position - transform.position).normalized;
        GameObject proj = Instantiate(ProjectilePrefab,transform.position, Quaternion.identity);
        proj.GetComponent<Projectile>().Init(dir, ProjectileDamage);
    }
}
