using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerCombat : MonoBehaviour
{

    [Header("Nasic Combat Attributes")]
    public float MeleeAttackRange = 0.4f;
    public float ParryRange = 0.3f;

    public int MeleeDamage = 50;
    public int AdrenalinDamage = 2000;
    public int max_hp = 1000;
    private float hp;
    public float naturalDrainRate = 2f;

    public float naturalHealRate = 1f;
    public float baseAttackCooldown = 1f;
    public float AdrenalinAttackCooldown = 0.25f;
    [HideInInspector] public bool room_cleared = false;

    [Header("References")]
    public LayerMask EnemyLayers;
    public LayerMask ProjectileLayers;
    public GameObject LightSpherePrefab;
    
    public Transform SideMeleePoint;
    public Transform UpMeleePoint;
    public Transform DownMeleePoint;
    public CharacterController controller;

    private GameObject LightSphere = null;

    private Vector2 InputVector;

    private bool ActionOnCooldown = false;



    private int iFrames = 0;

    private bool dead = false;

    [Header("Actions")]
    [SerializeField] private InputActionReference MoveAction;
    [SerializeField] private InputActionReference LShoulderAction;
    [SerializeField] private InputActionReference RShoulderAction;
    [SerializeField] private InputActionReference LTriggerAction;
    [SerializeField] private InputActionReference RTriggerAction;


    [HideInInspector] public bool light_dropped = false;

    private Vector3 AttackPoint = new Vector3();
    private Vector3 ParryPoint = new Vector3();


    ItemManager itemManager;
    
    private void Start()
    {
        itemManager = GameObject.FindGameObjectWithTag("ItemManager").GetComponent<ItemManager>();
        itemManager.UpdateItems();
    }

    public void ReloadItems()
    {
        hp = itemManager.max_hp;
    }

    void FixedUpdate()
    {
        if(iFrames > 0)
        {
            iFrames--;
        }

        if(!room_cleared && !dead)
        {
            if (!light_dropped)
            {
                if(hp + itemManager.HealRate * Time.fixedDeltaTime < itemManager.max_hp)
                {
                    hp += itemManager.HealRate * Time.fixedDeltaTime;
                }
                else
                {
                    hp = itemManager.max_hp;
                }
            }
            else
            {
                if(hp - itemManager.DrainRate * Time.fixedDeltaTime > 2)
                {
                    hp -= itemManager.DrainRate * Time.fixedDeltaTime;
                }
                else
                {
                    hp = 2;
                }
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

        if(Mathf.Abs(InputVector.x) > 0.3 || Mathf.Abs(InputVector.y) > 0.3 )
        {
            if(Mathf.Abs(InputVector.y) < 0.7)
            {
                AttackPoint = SideMeleePoint.position;
                ParryPoint = SideMeleePoint.position;
                print("Sideways Swing, " + InputVector);
            }
            else if(InputVector.y > 0) 
            {
                AttackPoint = UpMeleePoint.position;
                ParryPoint = UpMeleePoint.position;
                print("Upwards Swing, " + InputVector);
            }

            else if(InputVector.y < 0)
            {
                AttackPoint = DownMeleePoint.position;
                ParryPoint = DownMeleePoint.position;
                print("Downwards Swing, " + InputVector);
            }
        }
        else
        {
            AttackPoint = SideMeleePoint.position;
            ParryPoint = SideMeleePoint.position;
            print("Sideways Swing, No Input, " + InputVector);
        }

        Collider2D[] ProjectilesInParryRange = Physics2D.OverlapCircleAll(ParryPoint, ParryRange, ProjectileLayers);
        foreach(Collider2D projectile in ProjectilesInParryRange)
        {
            if(!projectile.GetComponent<Projectile>()) continue;
            
            projectile.GetComponent<Projectile>().EnterParryRange();
        }
    }

    void OnMelee()
    {
        if(!ActionOnCooldown)
        {
            if (light_dropped)
            {
                StartCoroutine(ActionCooldown(AdrenalinAttackCooldown));
            }
            else
            {
                StartCoroutine(ActionCooldown(baseAttackCooldown));
            }
            bool pogo = false;

            if(AttackPoint == DownMeleePoint.position)
            {
                pogo = true;
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
                    enemy.GetComponent<Enemy>().takeDamage(itemManager.MeleeDamage);
                }
                else
                {
                    enemy.GetComponent<Enemy>().takeDamage(AdrenalinDamage);
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
                if(itemManager.parry_leech_percent > 0)
                {
                    if(hp + projectile.GetComponent<Projectile>()._damage * itemManager.parry_leech_percent < max_hp)
                    {
                        hp += projectile.GetComponent<Projectile>()._damage * itemManager.parry_leech_percent;
                    }
                    else
                    {
                        hp = max_hp;
                    }
                }
            } 
        }
    }

    public void takeDamage(int damage, Vector2 dir)
    {
        Debug.Log("takeDamage aufgerufen, HP: " + hp + ", Schaden: " + damage + ", dead: " + dead + ", iFrames: " + iFrames);

        if (!dead)
        { 
            if(hp - damage > 0)
            {
                hp -= damage;
            }
            else
            {
                Debug.Log("ELSE BRANCH - DIE WIRD AUFGERUFEN");
                hp = 0;
                Die();
            }
            
            SetIFrames(5);
            
            if (!light_dropped && dir != new Vector2(0,0))
            {
                dropLight(damage,dir);
            }
        }
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

    public void Die()
    {
        Debug.Log("DIE WURDE AUFGERUFEN");
        dead = true;
        hp = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

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
                collision.GetComponent<Enemy>().takeDamage(itemManager.MeleeDamage);
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
