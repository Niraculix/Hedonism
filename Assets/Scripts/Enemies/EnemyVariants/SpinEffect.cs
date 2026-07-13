using UnityEngine;

public class SpinEffect : MonoBehaviour
{
    [SerializeField] private Transform bodyStar;
    [SerializeField] private Transform bodyInner;

    [SerializeField] private float spinSpeed = 180f; // Grad pro Sekunde

    void Update()
    {
        bodyStar.Rotate(0, 0, spinSpeed * Time.deltaTime);
        bodyInner.Rotate(0, 0, -spinSpeed * Time.deltaTime);
    }
}