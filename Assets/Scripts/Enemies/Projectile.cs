using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector2 _direction;
    private float speed = 8;
    private bool _isParried = false;
    private int _damage;

    private Vector3 _initPos = new Vector3();

    public void Init(Vector2 direction, int damage)
    {
        _direction = direction.normalized;
        _damage = damage;
        _initPos = transform.position;
    }

    // Wird vom ParryController aufgerufen
    public void Parry()
    {
        print("Parried");
        _isParried = true;
        // Richtung umkehren zurück zur Quelle
        _direction = (transform.position - _initPos).normalized;
        // Ein bisschen schneller nach Parry
        speed *= 1.5f;
    }

    private void FixedUpdate()
    {
        transform.Translate(_direction * speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Trifft Spieler (nur wenn nicht parriert)
        if (!_isParried && other.CompareTag("Player"))
        {
            Debug.Log("Spieler getroffen!");
            other.GetComponent<PlayerCombat>().takeDamage(_damage);
            Destroy(gameObject);
            return;
        }

        // Trifft Gegner nach Parry
        if (_isParried && other.CompareTag("Enemy"))
        {
            Debug.Log("Gegner getroffen!");
            other.GetComponent<Enemy>().takeDamage(_damage);
            Destroy(gameObject);
            return;
        }

        // Wand oder Boden
        if (other.CompareTag("Environment"))
        {
            Destroy(gameObject);
            return;
        }
    }
}
