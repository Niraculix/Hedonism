using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class Projectile : MonoBehaviour
{
    private Vector2 _direction;
    private float speed = 8;
    private bool _isParried = false;
    [HideInInspector] public int _damage;

    private Vector3 _initPos = new Vector3();

    // Scale-up settings
    [SerializeField] private float growDuration = 0.5f;   // seconds
    [SerializeField] private Vector3 startScale = new Vector3(0.5f, 0.5f, 1f);
    [SerializeField] private Vector3 endScale = new Vector3(1f, 1f, 1f);
    private float _spawnTime;
    AudioManager audioManager = AudioManager.Instance;

    private void Awake()
    {
        // Ensure we start at the intended scale
        transform.localScale = startScale;
        _spawnTime = Time.time;
        GetComponent<Light2D>().color = new Color(255,95,95);

        StartCoroutine(ScaleOverTime());
    }
    private IEnumerator ScaleOverTime()
    {
        float elapsed = 0f;

        while (elapsed < growDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / growDuration);
            t = Mathf.SmoothStep(0f, 1f, t); // optional easing

            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }

        // Ensure final scale is exact
        transform.localScale = endScale;
    }
    public void Init(Vector2 direction, int damage)
    {
        _direction = direction.normalized;
        _damage = damage;
        _initPos = transform.position;

        audioManager.Play(audioManager.BulletShootSound);
    }

    // Wird vom ParryController aufgerufen
    public void Parry()
    {
        print("Parried");
        _isParried = true;

        audioManager.Play(audioManager.parrySound,1,Random.Range(0.9f, 1.1f));
        
        GetComponent<Light2D>().color = Color.white;
        _direction = (_initPos - transform.position).normalized;
        speed *= 2f;


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
            if(other.GetComponent<PlayerCombat>().GetIFrames() == 0)
            {
                Debug.Log("Spieler getroffen!");
                other.GetComponent<PlayerCombat>().takeDamage(_damage, _direction);
                other.GetComponent<CharacterController>().Knockback(_direction,_damage);
                audioManager.Play(audioManager.BulletHitSound,1,Random.Range(0.9f, 1.1f));
                Destroy(gameObject);
            }
            return;
        }

        // Trifft Gegner nach Parry
        if (_isParried && other.CompareTag("Enemy"))
        {
            Debug.Log("Gegner getroffen!");
            other.GetComponent<Enemy>().takeDamage(_damage);
                audioManager.Play(audioManager.BulletHitSound,1,Random.Range(0.9f, 1.1f));
            Destroy(gameObject);
            return;
        }

        // Wand oder Boden
        if (other.CompareTag("Environment"))
        {
            Destroy(gameObject);
                audioManager.Play(audioManager.BulletHitSound,1,Random.Range(0.7f, 1f));
            return;
        }
    }

    public IEnumerator EnterParryRange()
    {
        audioManager.Play(audioManager.EnterParryRangeSound,1,Random.Range(0.9f, 1.1f));

        for(var i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(0.1f);
            GetComponent<Light2D>().intensity += 0.5f;
        }

        yield return new WaitForSeconds(0.5f);

        for(var i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(0.1f);
            GetComponent<Light2D>().intensity -= 0.5f;
        }
    }
}
