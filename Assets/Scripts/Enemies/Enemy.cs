using UnityEngine;
using UnityEngine.Rendering;

public class Enemy : MonoBehaviour
{
    public int hp = 30;

    public int contact_dmg = 5;

    private int IFrames = 0;

    [HideInInspector] public bool active = false;
    [HideInInspector] public bool LogicEnabled = true;

    public GameObject FloatingNumberPrefab;

    public Canvas canvas;
    public GameObject NumberSpawnPoint;

    

    public void Start()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Rigidbody2D>().Sleep();
        LogicEnabled = false;
    }

    public void EnableLogic()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<BoxCollider2D>().enabled = true;
        GetComponent<Rigidbody2D>().WakeUp();
        LogicEnabled = true;

        //STARTUP ANIMATION LOGIC HERE


    }

    void FixedUpdate()
    {
        if(active && !LogicEnabled)
        {
            EnableLogic();
        }

        if(IFrames > 0)
        {
            IFrames--;
        }
    }

    public void takeDamage(int damage)
    {
        if(IFrames <= 0)
        {   
            hp -= damage;
            print("Damaged " + gameObject.name + ", HP Now: " + hp);

            SpawnDamageNumber((float)damage);

            IFrames = 5;

            if(hp <= 0)
            {
                Die();
            }
        }
    }

    void Die()
    {
        GameObject.FindGameObjectWithTag("Room").GetComponent<RoomDefinition>().EnemyKilled();

        if(GameObject.FindGameObjectWithTag("ItemManager").GetComponent<ItemManager>().DashResetOnKill)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>().RegainDash();
        }
        
        Destroy(gameObject);
    }

    void SpawnDamageNumber(float dmg)
    {
        GameObject NewNumber = Instantiate(FloatingNumberPrefab, canvas.transform);
        float randf = Random.Range(-5,5) * (Random.Range(0,10) / 10f);
        NewNumber.transform.position = NumberSpawnPoint.transform.position;
        NewNumber.transform.Translate(new Vector2(randf,0));
        NewNumber.GetComponent<FloatingDamageNumber>().Init(dmg);
    }
}
