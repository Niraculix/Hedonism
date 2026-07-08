using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using Unity.Mathematics;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float moveSpeed = 0f;

    float horizontalMove = 0f;

    [HideInInspector] public Vector2 movementVector;
    bool jumping = false;
    bool dashing = false;

    void OnMove(InputValue value)
    {
        if(Mathf.Abs(value.Get<Vector2>().x) > 0.25)
        {
            movementVector = value.Get<Vector2>();
        }
        else
        {
            movementVector = new Vector2(0,0);
        }
        
    }

    void OnJump()
    {
        jumping = true;
    }

    void OnDash()
    {
        dashing = true;
        GetComponent<PlayerCombat>().SetIFrames(10);
    }

    void FixedUpdate()
    {
        horizontalMove = movementVector.x * moveSpeed;
        controller.Move(horizontalMove * Time.fixedDeltaTime, jumping, dashing);
        jumping = false;
        dashing = false;
        
    }
}
