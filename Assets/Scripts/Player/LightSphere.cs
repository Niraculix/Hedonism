using System.Collections;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class LightSphere : MonoBehaviour
{
    public Vector2 dir;
    bool pickup_ready;
    [SerializeField] private Transform GroundCheck;
    [SerializeField] private LayerMask GroundLayers;
    [SerializeField] private Rigidbody2D rb;

    public float floating_height;

    bool floating;
    bool grounded;


    public void Init(Vector2 dir, int dmg)
    {
        Vector2 force = new Vector2(0, Random.Range(10,50) * dmg);
        if(dir.x > 0)
        {
            force.x = 100 * dmg;
        }

        else
        {
            force.x = -100 * dmg;
        } 

        rb.AddForce(force);
        StartCoroutine(pickupCooldown());

    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(pickup_ready) {
            collision.GetComponent<PlayerCombat>().pickupLight();
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(GroundCheck.position, .05f, GroundLayers);

        for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				grounded = true;
			}
		}

        if(rb.linearVelocity.x != 0)
        {
            rb.linearVelocityX *= 0.99f;
        }

        if(grounded && !floating)
        {
            StartCoroutine(ball_floating());
            rb.gravityScale = 0.25f;
        }
    }

    IEnumerator pickupCooldown()
    {
        yield return new WaitForSeconds(0.5f);
        pickup_ready = true;
    }

    IEnumerator ball_floating()
    {
        floating = true;
        rb.gravityScale *= -1;
        print("change grav");
        yield return new WaitForSeconds(floating_height);

        if(grounded == true)
        {
            StartCoroutine(ball_floating());
        }
    }
}
