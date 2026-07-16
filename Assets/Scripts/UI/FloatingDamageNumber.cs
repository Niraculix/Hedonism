using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FloatingDamageNumber : MonoBehaviour
{
    Color color;
    public int Lifetime = 150;
    int currLifetime;
    public float FloatingSpeed = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currLifetime = Lifetime;

        int randi = Random.Range(0,100);
        color = new Color(255,randi,randi);

        GetComponent<TextMeshProUGUI>().color = Color.white;
        GetComponent<Light2D>().color = Color.red;
    }

    public void Init(float dmg)
    {
        GetComponent<TextMeshProUGUI>().text = $"{dmg}";
        print($"Spawned! Dmg: {dmg}, Position: {transform.position}")
;    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(currLifetime > 0)
        {
            currLifetime--;
        }
        else
        {
            print("despawn");
            Destroy(gameObject);
        }

        transform.Translate(new Vector2(0,FloatingSpeed * Time.fixedDeltaTime));
        if(currLifetime < Lifetime / 2)
        {
            GetComponent<TextMeshProUGUI>().alpha -= 1 / (Lifetime / 2f);
            GetComponent<Light2D>().intensity -= 0.1f;
        }
    }
}
