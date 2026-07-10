using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FloatingDamageNumber : MonoBehaviour
{
    Color color;
    public int Lifetime = 200;
    int currLifetime;
    public float FloatingSpeed = 10;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currLifetime = Lifetime;

        int randi = Random.Range(0,100);
        color = new Color(255,randi,randi);

        GetComponent<TextMeshProUGUI>().color = color;
        GetComponent<Light2D>().color = color;
    }

    void Init()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(currLifetime < 0)
        {
            currLifetime--;
        }
        else
        {
            Destroy(gameObject);
        }

        transform.Translate(new Vector2(0,FloatingSpeed * Time.fixedTime));
        if(currLifetime < Lifetime / 2)
        {
            GetComponent<TextMeshProUGUI>().alpha -= 1 / (Lifetime / 2);
        }
    }
}
