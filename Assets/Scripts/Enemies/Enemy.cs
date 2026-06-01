using UnityEngine;
using UnityEngine.Rendering;

public class Enemy : MonoBehaviour
{
    public int hp = 30;

    public int contact_dmg = 5;

    public void takeDamage(int damage)
    {
        hp -= damage;
        print("Damaged " + gameObject.name + ", HP Now: " + hp);

        if(hp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
