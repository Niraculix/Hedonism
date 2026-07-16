using UnityEngine;
using DG.Tweening;

public class PlantWobble : MonoBehaviour
{
    [Header("References")]
    public Transform sprite;

    [Header("Settings")]
    public float bendAmount = 12f;
    public float wobbleDuration = 0.08f;

    private bool wobbling = false;

    private Quaternion originalRotation;

    private void Start()
    {
        originalRotation = sprite.localRotation;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (wobbling)
            return;

        if (other.CompareTag("Player"))
        {
            Wobble(other.transform);
        }
    }

    private void Wobble(Transform player)
    {
        wobbling = true;

        // Bestimmt, von welcher Seite der Spieler kommt
        float direction = player.position.x < transform.position.x ? 1f : -1f;

        Sequence wobble = DOTween.Sequence();

        wobble.Append(
            sprite.DOLocalRotate(
                new Vector3(0, 0, bendAmount * direction),
                wobbleDuration
            ).SetEase(Ease.OutQuad)
        );

        wobble.Append(
            sprite.DOLocalRotate(
                new Vector3(0, 0, -bendAmount * 0.5f * direction),
                wobbleDuration * 1.2f
            ).SetEase(Ease.InOutQuad)
        );

        wobble.Append(
            sprite.DOLocalRotate(
                new Vector3(0, 0, bendAmount * 0.25f * direction),
                wobbleDuration
            ).SetEase(Ease.InOutQuad)
        );

        wobble.Append(
            sprite.DOLocalRotate(
                originalRotation.eulerAngles,
                wobbleDuration * 1.5f
            ).SetEase(Ease.OutQuad)
        );
        
        wobble.OnComplete(() =>
        {
            wobbling = false;
        });
    }
}