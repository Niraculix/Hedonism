using UnityEngine;

public class EntityPlaceHolder : MonoBehaviour
{
    public float amplitude = 0.25f;
    public float speed = 2f;

    void Start()
    {
        GameObject backgroundBase = GameObject.Find("Background Base");

        if (backgroundBase != null)
        {
            transform.SetSiblingIndex(backgroundBase.transform.GetSiblingIndex() + 1);
        }
    }

    void LateUpdate()
    {
        Vector3 screenCenter = new Vector3(0.5f, 0.5f, 10f);
        Vector3 centerPosition = Camera.main.ViewportToWorldPoint(screenCenter);

        float yOffset = Mathf.Sin(Time.time * speed) * amplitude;

        transform.position = centerPosition + new Vector3(0f, yOffset, 0f);
    }
}