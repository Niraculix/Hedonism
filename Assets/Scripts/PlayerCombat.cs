using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    private Vector2 InputVector;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnMove(InputValue value)
    {
        InputVector = value.Get<Vector2>();
    }


    void OnMelee()
    {
        if(InputVector.x < 0.3 && InputVector.x > -0.3 )
        {
            if(InputVector.y > 0) 
            {
                print("Upwards Swing");
            }

            if(InputVector.y < 0)
            {
                print("Downwards Swing");
            }

            if(InputVector == new Vector2(0,0))
            {
                print("Sideways Swing");
            }
            
        }
        else
        {
            print("Sideways Swing");
        }
    }
}
