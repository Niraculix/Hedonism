using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class PlayerCombat : MonoBehaviour
{

    [Header("Nasic Combat Attributes")]
    public float MeleeAttackRange = 0.4f;
    public float ParryRange = 0.3f;

    [Header("visuals")]
    public float LightIntensity = 2f;
    public float FreezeDuration = 3;
    public float FreezeDelay = 0.2f;

    public int MeleeDamage = 50;
    public int AdrenalinDamage = 2000;
    public int max_hp = 1000;
    [HideInInspector] public float hp;
    public float naturalDrainRate = 4f;

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


    private Vector2 InputVector;

    private bool ActionOnCooldown = false;

    private int iFrames = 0;

    private bool dead = false;

    private bool draining = false;

    private int attackDirection = 0;

    [Header("Actions")]
    [SerializeField] private InputActionReference MoveAction;
    [SerializeField] private InputActionReference LShoulderAction;
    [SerializeField] private InputActionReference RShoulderAction;
    [SerializeField] private InputActionReference LTriggerAction;
    [SerializeField] private InputActionReference RTriggerAction;


    [HideInInspector] public bool light_dropped = false;

    private Vector3 AttackPoint = new Vector3();
    private Vector3 ParryPoint = new Vector3();

    private Light2D LightComponent;
    private string gameOverSceneName = "GameOverScene";


    ItemManager itemManager;
    GameManager gameManager;
    AudioManager audioManager = AudioManager.Instance;

    
    private void Start()
    {
        itemManager = GameObject.FindGameObjectWithTag("ItemManager").GetComponent<ItemManager>();
        itemManager.UpdateItems();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        LightComponent = GetComponent<Light2D>();

        if(!gameManager.DungeonGenerationOn) hp = itemManager.max_hp;
    }

    public void ReloadItems()
    {
        if(itemManager.max_hp > max_hp)
        {
            hp += itemManager.max_hp - max_hp;
        }

        else if(hp > itemManager.max_hp)
        {
            hp = itemManager.max_hp;
        }
        max_hp = itemManager.max_hp;
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
                if (draining)
                {
                    draining = false;
                    StartCoroutine(Heal());
                }
            }
            else
            {
                if (!draining)
                {
                    draining = true;
                    StartCoroutine(Drain());
                }
            }

            LightComponent.intensity = hp / max_hp * LightIntensity;
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
                attackDirection = 0;
                AttackPoint = SideMeleePoint.position;
                ParryPoint = SideMeleePoint.position;
            }
            else if(InputVector.y > 0) 
            {
                attackDirection = 1;
                AttackPoint = UpMeleePoint.position;
                ParryPoint = UpMeleePoint.position;
            }

            else if(InputVector.y < 0)
            {
                attackDirection = 2;
                AttackPoint = DownMeleePoint.position;
                ParryPoint = DownMeleePoint.position;
            }
        }
        else
        {
            attackDirection = 0;
            AttackPoint = SideMeleePoint.position;
            ParryPoint = SideMeleePoint.position;
        }

        Collider2D[] ProjectilesInParryRange = Physics2D.OverlapCircleAll(ParryPoint, ParryRange, ProjectileLayers);
        foreach(Collider2D projectile in ProjectilesInParryRange)
        {
            if(!projectile.GetComponent<Projectile>()) continue;
            
            StartCoroutine(projectile.GetComponent<Projectile>().EnterParryRange());
        }
    }

    void OnMelee()
    {
        if(GameObject.FindGameObjectWithTag("PauseUI").GetComponent<PauseMenu>().IsPaused) return;
        if(!ActionOnCooldown)
        {
            controller.TriggerAttackAnimation(attackDirection);
            audioManager.Play(audioManager.attackSound);

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

            if(AttackPoint == SideMeleePoint.position)
            {
                hitEnemies = Physics2D.OverlapBoxAll(AttackPoint, new Vector2(MeleeAttackRange * 7f, MeleeAttackRange * 3f), 0, EnemyLayers);
            }
            else
            {
                hitEnemies = Physics2D.OverlapBoxAll(AttackPoint, new Vector2(MeleeAttackRange * 2f, MeleeAttackRange * 7f), EnemyLayers);
            }

            
            foreach(Collider2D enemy in hitEnemies)
            {
                if(!enemy.GetComponent<Enemy>()) return;
                audioManager.Play(audioManager.EnemyDamageSound,1,UnityEngine.Random.Range(0.9f, 1.1f));

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
            controller.TriggerKnockbackAnimation();

            if (hp - damage > 0)
            {
                hp -= damage;
            }
            else
            {
                hp = 0;
                StartCoroutine(Die());
            }

            audioManager.Play(audioManager.PlayerDMGSound,1,Random.Range(0.9f,1.1f));

            StartCoroutine(GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().FreezeGame(FreezeDuration, FreezeDelay));     
            
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

            audioManager.Play(audioManager.EnterAdrenalinSound);

			GetComponent<OrbitManager>().BuildOrbiters();
        }
    }

    public void pickupLight()

    {
        light_dropped = false;
        audioManager.Play(audioManager.ExitAdrenalinSound);
		GetComponent<OrbitManager>().BuildOrbiters();
    }


    public IEnumerator Die()
    {
        dead = true;
        hp = 0;
        audioManager.StopMusic();
        audioManager.Play(audioManager.gameOverSound);
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(gameOverSceneName);
    }
    
    IEnumerator ActionCooldown(float cooldownSec)
    {
        ActionOnCooldown = true;
        yield return new WaitForSeconds(cooldownSec);
        ActionOnCooldown = false;

    }

    IEnumerator Drain()
    {

        if(draining)
        {
            float drainDmg = (float)max_hp / 100 * itemManager.DrainRate;
            if(hp - drainDmg > 2)
            {
                
                hp -= drainDmg / 3;
            }
            else
            {
                hp = 2;
            }

        }
        yield return new WaitForSeconds(0.33f);

        if(draining) StartCoroutine(Drain());
    }

    IEnumerator Heal()
    {

        if(!draining)
        {
            float healAmount = (float)max_hp / 100 * itemManager.HealRate;
            if(hp + healAmount < itemManager.max_hp)
            {
                
                hp += healAmount / 3;
            }
            else
            {
                hp = itemManager.max_hp;
            }

        }
        yield return new WaitForSeconds(0.33f);

        if(!draining) StartCoroutine(Heal());
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

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(SideMeleePoint.position, new Vector2(MeleeAttackRange * 7f, MeleeAttackRange * 3f));
        Gizmos.DrawWireCube(DownMeleePoint.position, new Vector2(MeleeAttackRange * 2f, MeleeAttackRange * 7f));
        Gizmos.DrawWireCube(UpMeleePoint.position, new Vector2(MeleeAttackRange * 2f, MeleeAttackRange * 7f));

        Gizmos.color = Color.blue;

        Gizmos.DrawWireSphere(SideMeleePoint.position,ParryRange);
    }
}
