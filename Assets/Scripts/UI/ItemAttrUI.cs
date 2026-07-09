using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class ItemAttrUI : MonoBehaviour
{
    public GameObject AttrNameObj;
    public GameObject AttrValStartObj;
    public GameObject AttrValEndObj;

    public void Init(string name, string currValue, string newValue, Color color)
    {
        AttrNameObj.GetComponent<TextMeshProUGUI>().text = name;
        AttrValStartObj.GetComponent<TextMeshProUGUI>().text = currValue;
        AttrValEndObj.GetComponent<TextMeshProUGUI>().text = newValue;
        AttrValEndObj.GetComponent<TextMeshProUGUI>().color = color;

    }
}
