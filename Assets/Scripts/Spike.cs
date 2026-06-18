using UnityEngine;

public class Spike : MonoBehaviour
{
    public int damage = 1000; //Schaden hier anpassen büdde

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerCombat player = collision.GetComponent<PlayerCombat>();

        if (player != null)
        {
            Vector2 dir = ((Vector2)collision.transform.position - (Vector2)transform.position).normalized;
            player.takeDamage(damage, dir);     //knockback adden nico fragen enemy knockback
        }
    }
}