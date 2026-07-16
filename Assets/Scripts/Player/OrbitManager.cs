using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Erzeugt beim Start eine festgelegte Anzahl an Orb-Instanzen (aus orbPrefab)
/// und lässt sie gleichmäßig verteilt um einen Mittelpunkt kreisen.
/// </summary>
public class OrbitManager : MonoBehaviour
{
    private class Orbiter
    {
        public Transform target;
        public float currentAngle; // Grad
    }

    public Transform center;

    public GameObject orbPrefab;

    int orbCount;

    [Tooltip("Abstand der Orbs zum Mittelpunkt.")]
    public float radius = 2f;

    public float speed = 60f;

    private readonly List<Orbiter> orbiters = new List<Orbiter>();

    void Awake()
    {
        if (center == null) center = transform;
    }


    [ContextMenu("Rebuild Orbiters")]
    public void BuildOrbiters()
    {
        orbCount = GetComponent<CharacterController>().dashes_remaining;

        foreach (var o in orbiters)
        {
            if (o.target != null) Destroy(o.target.gameObject);
        }
        orbiters.Clear();

        if (orbPrefab == null || orbCount <= 0) return;

        float angleStep = 360f / orbCount;

        for (int i = 0; i < orbCount; i++)
        {
            GameObject go = Instantiate(orbPrefab, transform.parent);
            orbiters.Add(new Orbiter
            {
                target = go.transform,
                currentAngle = i * angleStep
            });
        }
    }

    void FixedUpdate()
    {
        Vector3 centerPos = center.position;

        foreach (var o in orbiters)
        {
            if (o.target == null) continue;

            o.currentAngle += speed * Time.fixedDeltaTime;
            float rad = o.currentAngle * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * radius;
            o.target.position = centerPos + offset;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blueViolet;
        Gizmos.DrawWireSphere(center.position,radius);
    }
}