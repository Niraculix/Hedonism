using UnityEngine;

public class AutodestroyAttackVFX : MonoBehaviour
{
    /// t visibility vfx

    [SerializeField] private float lifetime = 0.2f;


    void Start()
    {
        Destroy(gameObject,lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
