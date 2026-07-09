using UnityEngine;

public class ItemDescUI : MonoBehaviour
{
    CircleCollider2D triggerCollider;
    [SerializeField] private LayerMask playerLayer;
    public GameObject DescUI;

    void Start()
    {
        triggerCollider = GetComponent<CircleCollider2D>();
        DescUI.SetActive(false);
    }

    void FixedUpdate()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, triggerCollider.radius, playerLayer);
        DescUI.SetActive(false);

        foreach(Collider2D other in hits)
        {
            if (other.GetComponent<CharacterController>())
            {
                DescUI.SetActive(true);
            }
        }
    }
}
