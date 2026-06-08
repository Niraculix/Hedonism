using System.Collections;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using Unity.Mathematics;

public class CharacterController : MonoBehaviour
{
	[SerializeField] private float speed = 10f;
	[SerializeField] private float m_JumpForce = 1400f;
	[SerializeField] private float m_DashForce = 600f;
	[SerializeField] private float m_PogoForce = 600f;
	public float m_DashCooldown = 0.1f;

	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;
	[SerializeField] private LayerMask m_WhatIsGround;
	[SerializeField] private Transform m_GroundCheck;

	[SerializeField] private PhysicsMaterial2D PlayerMat;
	[SerializeField] private PhysicsMaterial2D BouncyPlayerMat;



	const float k_GroundedRadius = .2f;
	private bool m_Grounded;
	private bool m_DashOnCooldown;
	private bool m_DashAvailable;
	private bool can_Move = true;
	private Rigidbody2D m_Rigidbody2D;
	public bool m_FacingRight = true;
	private Vector3 m_Velocity = Vector3.zero;

	public int CoyoteTime;

	private int CoyoteTimeFrames;

	[SerializeField] private InputActionReference jumpAction;
	private bool jumpInputReleased;

	private Vector2 InputVector;


	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }


	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();
	}

	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;
		jumpInputReleased = !jumpAction.action.IsPressed();

		//print(jumpInputReleased);

		// The player is grounded if a circlecast to the groundcheck position hits anything groundy
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();

				if(m_DashOnCooldown == false && m_DashAvailable == false)
				{
					m_DashAvailable = true;
					//print("Dash Available");
				}

				ResetCoyoteTime(CoyoteTime);
			}

		}

		if(CoyoteTimeFrames > 0)
		{
			CoyoteTimeFrames--;
		}

	}


	public void Move(float move, bool jump, bool dash)
	{
		if(can_Move)
		{
			
			Vector3 targetVelocity = new Vector2(move * speed, m_Rigidbody2D.linearVelocity.y);
			
			// smoothing out and applying it
			m_Rigidbody2D.linearVelocity = Vector3.SmoothDamp(m_Rigidbody2D.linearVelocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !m_FacingRight)
			{
				Flip();
			}
			else if (move < 0 && m_FacingRight)
			{
				Flip();
			}
			// Player springt
			if (jump)
			{
				if(m_Grounded || CoyoteTimeFrames > 0)
				{
				m_Grounded = false;

				m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
				}
					
			}
		}

		// auf y achse ausbremsen, wenn jump action released wurde
		if(jumpInputReleased && m_Rigidbody2D.linearVelocity.y > 0)
			{
				m_Rigidbody2D.linearVelocity = new Vector2(m_Rigidbody2D.linearVelocity.x, m_Rigidbody2D.linearVelocity.y / 1.5f);
			}

		if (m_DashAvailable && dash)
		{
			m_Grounded = false;
			m_DashAvailable = false;

			StartCoroutine(DashCooldown());
			StartCoroutine(DashTime(0.1f));

			if(Mathf.Abs(InputVector.x) < 0.3)
			{
				InputVector.x = 0;
			}
			if(Mathf.Abs(InputVector.y) < 0.3 )
			{
				InputVector.y = 0;
			}

			
			if(InputVector ==  new Vector2(0,0))
			{
				print(InputVector);
				if(m_FacingRight)
				{
					InputVector = new Vector2(1,0);
				}
				else
				{
					InputVector = new Vector2(-1,0);
				}
			}

			m_Rigidbody2D.AddForce(InputVector * m_DashForce);
		}
	}


	private void Flip()
	{
		// flip the player
		m_FacingRight = !m_FacingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	IEnumerator DashCooldown()
	{
		m_DashOnCooldown = true;

		yield return new WaitForSeconds(m_DashCooldown);

		ResetDash();

	}

	IEnumerator DashTime(float time)
	{
		m_Rigidbody2D.sharedMaterial = BouncyPlayerMat;
		StartCoroutine(DisableMoveForSec(time / 2));
		GetComponent<TrailRenderer>().emitting = true;

		yield return new WaitForSeconds(time);
		
		m_Rigidbody2D.sharedMaterial = PlayerMat;
		GetComponent<TrailRenderer>().emitting = false;
	}

	public void ResetDash()
	{
		m_DashOnCooldown = false;
	}

	public void ResetCoyoteTime(int i)
	{
		CoyoteTimeFrames = i;
	}

	void OnMove(InputValue value)
	{
		InputVector = value.Get<Vector2>();
	}

	public void Pogo()
	{
		print("Pogo");
		m_Rigidbody2D.linearVelocityY = 0;
		m_Rigidbody2D.AddForce(new Vector2(0,m_PogoForce));
		ResetDash();
	}

	public void Knockback(Vector2 dir, int dmg)
	{
		m_Rigidbody2D.linearVelocityX = 0;
		m_Rigidbody2D.AddForce(dir * dmg * 500);
		StartCoroutine(MoveStun(0.4f));
	}

	IEnumerator MoveStun(float sec)
	{
		can_Move = false;
		yield return new WaitForSeconds(sec);
		can_Move = true;
	}

	IEnumerator DisableMoveForSec(float sec)
	{
		if(can_Move == true)
		{
			can_Move = false;
		}
		yield return new WaitForSeconds(sec);
		can_Move = true;
	}

    public void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(m_GroundCheck.position, k_GroundedRadius);
    }
}