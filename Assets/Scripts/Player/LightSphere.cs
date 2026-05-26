using System.Collections;
using UnityEngine;

public class LightSphere : MonoBehaviour
{
    public Vector2 dir;
    bool pickup_ready;

    public void Init(Vector2 dir, int dmg)
    {
        Vector2 force = new Vector2(0, Random.Range(10,50) * dmg);
        if(dir.x > 0)
        {
            force.x = 200 * dmg;
        }

        else
        {
            force.x = -200 * dmg;
        } 

        GetComponent<Rigidbody2D>().AddForce(force);
        StartCoroutine(pickupCooldown());

    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(pickup_ready) {
            collision.GetComponent<PlayerCombat>().pickupLight();
            Destroy(gameObject);
        }
    }

    IEnumerator pickupCooldown()
    {
        yield return new WaitForSeconds(0.5f);
        pickup_ready = true;
    }
}
