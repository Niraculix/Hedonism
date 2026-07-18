using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class Enemy : MonoBehaviour
{
    public int hp = 30;

    public int contact_dmg = 5;

    private int IFrames = 0;

    [HideInInspector] public bool active = false;
    [HideInInspector] public bool LogicEnabled = true;

    public GameObject animationObject;

    public GameObject NumberSpawnPoint;

    public GameObject FloatingNumberPrefab;

    public Canvas canvas;

    AudioManager audioManager = AudioManager.Instance;

    

    public void Start()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        if(animationObject != null) animationObject.SetActive(false);
        if(GetComponent<SpriteRenderer>()) GetComponent<SpriteRenderer>().enabled = false;
        if(GetComponent<Light2D>()) GetComponent<Light2D>().intensity = 0f;
        GetComponent<Rigidbody2D>().Sleep();
        LogicEnabled = false;
    }

    public void EnableLogic()
    {
        if(GetComponent<PlatformerEnemyAI>()) print("Logic enabled");
        if(animationObject != null) animationObject.SetActive(true);
        if(GetComponent<SpriteRenderer>()) GetComponent<SpriteRenderer>().enabled = true;
        if(GetComponent<Light2D>()) GetComponent<Light2D>().intensity = 1f;
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

        if (transform.position.y < GameObject.FindGameObjectWithTag("Room").GetComponent<RoomDefinition>().KillBoxY)
        {
            GameObject.FindGameObjectWithTag("Room").GetComponent<RoomDefinition>().EnemyKilled();
            LogicEnabled = false;
        }

        if(transform.localScale.x < 0)
        {
            canvas.transform.localScale = new Vector2(-1.428571f,1.428571f);
        }
        else
        {
            canvas.transform.localScale = new Vector2(1.428571f,1.428571f);
        }
    }

    public void takeDamage(int damage)
    {
        if(IFrames <= 0)
        {   
            hp -= damage;
            print("Damaged " + gameObject.name + ", HP Now: " + hp);

            IFrames = 5;

            SpawnDamageNumber(damage);

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

        LogicEnabled = false;
        audioManager.Play(audioManager.EnemyDeathSound);

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
