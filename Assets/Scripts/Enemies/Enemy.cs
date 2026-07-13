using UnityEngine;
using UnityEngine.Rendering;

public class Enemy : MonoBehaviour
{
    public int hp = 30;

    public int contact_dmg = 5;

    private int IFrames = 0;

    [HideInInspector] public bool active = false;
    [HideInInspector] public bool LogicEnabled = true;

    public GameObject animationObject;

    

    public void Start()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        if(animationObject != null) animationObject.SetActive(false);
        if(GetComponent<SpriteRenderer>()) GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Rigidbody2D>().Sleep();
        LogicEnabled = false;
    }

    public void EnableLogic()
    {
        if(GetComponent<PlatformerEnemyAI>()) print("Logic enabled");
        if(animationObject != null) animationObject.SetActive(true);
        if(GetComponent<SpriteRenderer>()) GetComponent<SpriteRenderer>().enabled = true;
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
            Destroy(gameObject);
        }
    }

    public void takeDamage(int damage)
    {
        if(IFrames <= 0)
        {   
            hp -= damage;
            print("Damaged " + gameObject.name + ", HP Now: " + hp);

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
        
    }
}
