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
    }

    void Update()
    {
        if (flyingEnemy.CurrentState == FlyingEnemy.EnemyState.Chase)
            visual.sprite = alertedSprite;
        else
            visual.sprite = idleSprite;
    }
}