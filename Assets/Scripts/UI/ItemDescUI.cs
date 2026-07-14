using System;
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
        DescUI.SetActive(true);
        foreach(Transform child in GetComponentsInChildren<Transform>())
        {
            if (child.GetComponent<ItemAttrUI>())
            {
                child.transform.localPosition = new Vector2(0, child.GetComponent<ItemAttrUI>().Y_Offset);
            }
        }

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
