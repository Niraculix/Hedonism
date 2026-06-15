using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class dashUI : MonoBehaviour
{
    // Update is called once per frame
    void FixedUpdate()
    {
        if(GameObject.FindGameObjectWithTag("Player") == null) return;

        CharacterController controller = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();

        GetComponent<TextMeshProUGUI>().text = controller.dashes_remaining + "/" + controller.MaxDashes;
    }


}
