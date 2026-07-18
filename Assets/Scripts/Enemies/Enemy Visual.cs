using UnityEngine;

public class EnemyVisual : MonoBehaviour
{
    [SerializeField] private SpriteRenderer visual;
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite alertedSprite;

    private FlyingEnemy flyingEnemy;

    void Start()
    {
        flyingEnemy = GetComponent<FlyingEnemy>();
        transform.localPosition = new Vector2(0,0);
    }

    void FixedUpdate()
    {

        if (flyingEnemy.CurrentState == FlyingEnemy.EnemyState.Chase)
        {
            visual.sprite = alertedSprite;
        }
        else
        {
            visual.sprite = idleSprite;
        }
        
    }
}