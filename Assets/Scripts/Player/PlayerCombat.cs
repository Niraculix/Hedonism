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

    public int MeleeDamage = 10;
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

    [SerializeField] private InputActionReference meleeAction;
    [SerializeField] private InputActionReference parryAction;
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


        if(meleeAction.action.IsPressed() && parryAction.action.IsPressed() && LTriggerAction.action.IsPressed() && RTriggerAction.action.IsPressed())
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

    }


    void OnMove(InputValue value)
    {
        InputVector = value.Get<Vector2>();
    }

    void OnMelee()
    {
        
        if(!ActionOnCooldown && !parryAction.action.IsPressed() && !LTriggerAction.action.IsPressed() && !RTriggerAction.action.IsPressed())
        {
            ActionCooldown(0.05f);
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

            Collider2D[] hitEnemies;

            if(AttackPoint == DownMeleePoint.position)
            {
                hitEnemies = Physics2D.OverlapBoxAll(AttackPoint, new Vector2(GetComponent<CapsuleCollider2D>().size.x, MeleeAttackRange * 2), 0, EnemyLayers);
            }
            else
            {
                hitEnemies = Physics2D.OverlapCircleAll(AttackPoint, MeleeAttackRange, EnemyLayers);
            }

            
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
                    controller.Pogo();
                    pogo = false;
                }
            }

            foreach(Collider2D projectile in hitProjectiles)
            {
                if(pogo)
                {
                    controller.Pogo();
                    pogo = false;
                }
            }
        }
    }

    void OnParry()
    {
        if (!ActionOnCooldown && !meleeAction.action.IsPressed() && !LTriggerAction.action.IsPressed() && !RTriggerAction.action.IsPressed())
        {
            ActionCooldown(0.05f);
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
        Gizmos.DrawWireSphere(SideMeleePoint.position, MeleeAttackRange);
        Gizmos.DrawWireCube(DownMeleePoint.position, new Vector2(GetComponent<CapsuleCollider2D>().size.x, MeleeAttackRange * 2));
        Gizmos.DrawWireSphere(UpMeleePoint.position, MeleeAttackRange);
    }
}
