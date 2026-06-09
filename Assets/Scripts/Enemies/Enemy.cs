using UnityEngine;
using UnityEngine.Rendering;

public class Enemy : MonoBehaviour
{
    public int hp = 30;

    public int contact_dmg = 5;

    private int IFrames = 0;

     private void FixedUpdate()
    {
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
        Destroy(gameObject);
    }
}
