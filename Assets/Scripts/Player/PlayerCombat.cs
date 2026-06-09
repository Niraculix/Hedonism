using System.Collections;
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

    public int MeleeDamage = 1000;
    public int max_hp = 1000;
    private float hp;
    public bool light_dropped = false;
    public bool room_cleared = false;
    public float natural_drain_rate = 1;

    public LayerMask EnemyLayers;
    public LayerMask ProjectileLayers;
    public GameObject LightSpherePrefab;
    public CharacterController controller;

    private GameObject LightSphere = null;

    private Vector2 InputVector;

    private bool ActionOnCooldown = false;

    private int iFrames = 0;

    [SerializeField] private InputActionReference MoveAction;
    [SerializeField] private InputActionReference LShoulderAction;
    [SerializeField] private InputActionReference RShoulderAction;
    [SerializeField] private InputActionReference LTriggerAction;
    [SerializeField] private InputActionReference RTriggerAction;
    
    private void Start()
    {
        hp = max_hp;
    }

    void FixedUpdate()
    {
        if(iFrames > 0)
        {
            iFrames--;
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


        if(LShoulderAction.action.IsPressed() && RShoulderAction.action.IsPressed() && LTriggerAction.action.IsPressed() && RTriggerAction.action.IsPressed())
        {
            if(!light_dropped){
                if(!ActionOnCooldown)
                {
                    ActionCooldown(0.1f);
                    if(controller.m_FacingRight)
                    {
                        dropLight(5, new Vector2(1,0));
                    }
                    else
                    {
                        dropLight(5, new Vector2(-1,0));
                    }
                }
            }
        }

        InputVector = MoveAction.action.ReadValue<Vector2>();
    }

    void OnMelee()
    {
        if(!ActionOnCooldown)
        {
            ActionCooldown(0.05f);
            bool pogo = false;
            Vector3 AttackPoint = new Vector3();
            Vector3 ParryPoint = new Vector3();
            if(InputVector.x > 0.3 && InputVector.x < -0.3 )
            {
                if(InputVector.y > 0) 
                {
                    AttackPoint = UpMeleePoint.position;
                    ParryPoint = UpMeleePoint.position;
                    print("Upwards Swing, " + InputVector);
                }

                else if(InputVector.y < 0)
                {
                    AttackPoint = DownMeleePoint.position;
                    pogo = true;
                    print("Downwards Swing");
                }

                else if(Mathf.Abs(InputVector.y) < 0.3)
                {
                    AttackPoint = SideMeleePoint.position;
                    ParryPoint = SideMeleePoint.position;
                    print("Sideways Swing");
                }
                
            }
            else
            {
                AttackPoint = SideMeleePoint.position;
                ParryPoint = SideMeleePoint.position;
                print("Sideways Swing");
            }

            Collider2D[] hitEnemies;

            if(AttackPoint == DownMeleePoint.position)
            {
                hitEnemies = Physics2D.OverlapBoxAll(AttackPoint, new Vector2(GetComponent<CapsuleCollider2D>().size.x * 1.5f, MeleeAttackRange * 3), 0, EnemyLayers);
            }
            else
            {
                hitEnemies = Physics2D.OverlapCircleAll(AttackPoint, MeleeAttackRange, EnemyLayers);
            }

            
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
                    controller.Pogo();
                    pogo = false;
                }
            }


            Collider2D[] hitProjectilesAttack = Physics2D.OverlapCircleAll(AttackPoint, MeleeAttackRange, ProjectileLayers);

            foreach(Collider2D projectile in hitProjectilesAttack)
            {
                if(pogo)
                {
                    controller.Pogo();
                    pogo = false;
                }
            }

            Collider2D[] hitProjectilesParry = Physics2D.OverlapCircleAll(ParryPoint, ParryRange, ProjectileLayers);

            foreach(Collider2D projectile in hitProjectilesParry)
            {
                projectile.GetComponent<Projectile>().Parry();
            } 
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

    IEnumerator ActionCooldown(float cooldownSec)
    {
        ActionOnCooldown = true;
        yield return new WaitForSeconds(cooldownSec);
        ActionOnCooldown = false;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
		if (collision.GetComponent<Enemy>())
		{
            if(iFrames <= 0)
            {
                Vector2 toEnemy = ((Vector2)transform.position - (Vector2)collision.transform.position).normalized;
                takeDamage(collision.GetComponent<Enemy>().contact_dmg, toEnemy);
                controller.Knockback(toEnemy, collision.GetComponent<Enemy>().contact_dmg);
            }
            else if(controller.dashing)
            {
                collision.GetComponent<Enemy>().takeDamage(MeleeDamage);
                print("dash dmg hit");
            }
                
		}
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

    public void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(SideMeleePoint.position, ParryRange);
        Gizmos.DrawWireCube(DownMeleePoint.position, new Vector2(GetComponent<CapsuleCollider2D>().size.x * 1.5f, MeleeAttackRange * 2));
        Gizmos.DrawWireSphere(UpMeleePoint.position, MeleeAttackRange);
    }
}
