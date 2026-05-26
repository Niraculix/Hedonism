using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{

    public Transform SideMeleePoint;
    public Transform UpMeleePoint;
    public Transform DownMeleePoint;

    public float MeleeAttackRange = 0.4f;
    public float ParryRange = 0.3f;

    public int MeleeDamage = 10;
    public int max_hp = 1000;
    private float hp;
    public bool light_dropped = false;
    public bool room_cleared = false;
    public float natural_drain_rate = 1;

    public LayerMask EnemyLayers;
    public LayerMask ProjectileLayers;
    public GameObject LightSpherePrefab;

    private GameObject LightSphere = null;

    private Vector2 InputVector;

    private int iFrames = 0;
    
    private void Start()
    {
        hp = max_hp;
    }

    void FixedUpdate()
    {
        if(iFrames > 0)
        {
            iFrames--;
            print("invincible : " + iFrames);
        }

        if(!room_cleared)
        {
            if (!light_dropped)
            {
                hp -= natural_drain_rate * Time.fixedDeltaTime;
            }
            else
            {
                hp -= natural_drain_rate * GetComponent<BerserkMode>().light_drain_mult * Time.fixedDeltaTime;
            }
        }
    }


    void OnMove(InputValue value)
    {
        InputVector = value.Get<Vector2>();
    }

    void OnMelee()
    {
        bool pogo = false;
        Vector3 AttackPoint = new Vector3();
        if(InputVector.x < 0.3 && InputVector.x > -0.3 )
        {
            if(InputVector.y > 0) 
            {
                AttackPoint = UpMeleePoint.position;
                print("Upwards Swing");
            }

            if(InputVector.y < 0)
            {
                AttackPoint = DownMeleePoint.position;
                pogo = true;
                print("Downwards Swing");
            }

            if(InputVector == new Vector2(0,0))
            {
                AttackPoint = SideMeleePoint.position;
                print("Sideways Swing");
            }
            
        }
        else
        {
            AttackPoint = SideMeleePoint.position;
            print("Sideways Swing");
        }

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(AttackPoint, MeleeAttackRange, EnemyLayers);
        Collider2D[] hitProjectiles = Physics2D.OverlapCircleAll(AttackPoint, MeleeAttackRange, ProjectileLayers);

        foreach(Collider2D enemy in hitEnemies)
        {
            if(!light_dropped) 
            {
                enemy.GetComponent<Enemy>().takeDamage(MeleeDamage);
            }
            else
            {
                enemy.GetComponent<Enemy>().takeDamage(MeleeDamage * GetComponent<BerserkMode>().dmg_mult);
            }

            if(pogo)
            {
                GetComponent<CharacterController>().Pogo();
                pogo = false;
            }
        }

        foreach(Collider2D projectile in hitProjectiles)
        {
            if(pogo)
            {
                GetComponent<CharacterController>().Pogo();
                pogo = false;
            }
        }

    }

    void OnParry()
    {
        Vector3 ParryPoint = new Vector3();
        if(InputVector.x < 0.3 && InputVector.x > -0.3 )
        {
            if(InputVector.y > 0) 
            {
                ParryPoint = UpMeleePoint.position;
                print("Upwards Parry");
            }

            if(InputVector.y < 0)
            {
                ParryPoint = DownMeleePoint.position;
                print("Downwards Parry");
            }

            if(InputVector == new Vector2(0,0))
            {
                ParryPoint = SideMeleePoint.position;
                //print("Sideways Parry");
            }
            
        }
        else
        {
            ParryPoint = SideMeleePoint.position;
            //print("Sideways Parry");
        }

        Collider2D[] hitProjectiles = Physics2D.OverlapCircleAll(ParryPoint, ParryRange, ProjectileLayers);

        foreach(Collider2D projectile in hitProjectiles)
        {
            projectile.GetComponent<Projectile>().Parry();
        } 
    }

    public void takeDamage(int damage, Vector2 dir)
    {
        hp -= damage;

        SetIFrames(5);
        dropLight(damage,dir);
    }

    void dropLight(int damage, Vector2 dir)
    {
        if(!light_dropped)
        {
            light_dropped = true;
            GameObject light = Instantiate(LightSpherePrefab,transform.position, Quaternion.identity);
            light.GetComponent<LightSphere>().Init(dir,damage);

            LightSphere = light;
        }
    }

    public void pickupLight()
    {
        LightSphere = null;
        light_dropped = false;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(SideMeleePoint.position, ParryRange);
    }

    public void SetIFrames(int i)
    {
        if(i > iFrames)
        {
            iFrames = i;
        }
    }

    public int GetIFrames()
    {
        return iFrames;
    }

    public float GetHp()
    {
        return hp;
    }
}
