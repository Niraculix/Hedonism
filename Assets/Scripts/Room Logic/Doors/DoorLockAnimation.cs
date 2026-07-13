using System.Collections;
using UnityEngine;

public class DoorLockAnimation : MonoBehaviour
{
    [Header("Referenzen")]
    [SerializeField] private Transform armCircle;
    [SerializeField] private Transform armPin;
    [SerializeField] private SpriteRenderer glowRed;

    [Header("Geschlossen-Positionen")]
    [SerializeField] private Vector2 armCircleClosedPos = new Vector2(0.54f, 1.7f);
    [SerializeField] private Vector2 armPinClosedPos = new Vector2(5.02f, 1.67f);

    [Header("Offen-Positionen (außerhalb sichtbar)")]
    [SerializeField] private Vector2 armCircleOpenPos = new Vector2(-3f, 1.7f);
    [SerializeField] private Vector2 armPinOpenPos = new Vector2(10f, 1.67f);

    [Header("Timing")]
    [SerializeField] private float animDuration = 0.4f;
    [SerializeField] private float glowDuration = 0.2f;

    private void Start()
    {
        // Start offen
        armCircle.localPosition = armCircleOpenPos;
        armPin.localPosition = armPinOpenPos;
        glowRed.enabled = false;
    }

    public void PlayLockAnimation()
    {
        StartCoroutine(LockRoutine());
    }

    public void PlayUnlockAnimation()
    {
        StartCoroutine(UnlockRoutine());
    }

    private IEnumerator LockRoutine()
    {
        float elapsed = 0f;
        Vector2 circleStart = armCircle.localPosition;
        Vector2 pinStart = armPin.localPosition;

        // Arme fahren zusammen
        while (elapsed < animDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / animDuration);
            armCircle.localPosition = Vector2.Lerp(circleStart, armCircleClosedPos, t);
            armPin.localPosition = Vector2.Lerp(pinStart, armPinClosedPos, t);
            yield return null;
        }

        // Einrasten - roter Glow
        glowRed.enabled = true;
        yield return new WaitForSeconds(glowDuration);
        glowRed.enabled = false;
    }

    private IEnumerator UnlockRoutine()
    {
        float elapsed = 0f;
        Vector2 circleStart = armCircle.localPosition;
        Vector2 pinStart = armPin.localPosition;

        while (elapsed < animDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / animDuration);
            armCircle.localPosition = Vector2.Lerp(circleStart, armCircleOpenPos, t);
            armPin.localPosition = Vector2.Lerp(pinStart, armPinOpenPos, t);
            yield return null;
        }
    }
}
