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
    public int max_hp = 50;
    private int hp;

    public LayerMask EnemyLayers;
    public LayerMask ProjectileLayers;

    private Vector2 InputVector;
    

    private void Start()
    {
        hp = max_hp;
    }


    void OnMove(InputValue value)
    {
        InputVector = value.Get<Vector2>();
    }


    void OnMelee()
    {
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

        foreach(Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().takeDamage(MeleeDamage);
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

    public void takeDamage(int damage)
    {
        
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(SideMeleePoint.position, ParryRange);
    }
}
