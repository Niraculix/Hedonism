using System.Collections;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using Unity.Mathematics;
using TMPro;

public class CharacterController : MonoBehaviour
{
	[Header("Movement Forces")]
	[SerializeField] private float speed = 10f;
	[SerializeField] private float m_JumpForce = 1400f;
	[SerializeField] private float m_PogoForce = 600f;
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;

	[Header("Dash")]
	[SerializeField] private float m_DashForce = 600f;
	public float m_DashCooldown = 2f;
	public float m_DashRechargeTime = 0.2f;
	[Range(1, 1.5f)] [SerializeField] private float DashYDamping;
	public int MaxDashes;

	[Header("Physics Materials")]
	[SerializeField] private PhysicsMaterial2D PlayerMat;
	[SerializeField] private PhysicsMaterial2D BouncyPlayerMat;

	[Header("Ground Referencing")]
	[SerializeField] private LayerMask m_WhatIsGround;
	[SerializeField] private Transform m_GroundCheck;


	private float grav;

	const float k_GroundedRadius = .08f;
	private bool m_Grounded;
	private bool m_DashOnCooldown;
	private bool can_Move = true;
	private Rigidbody2D m_Rigidbody2D;
	private Vector3 m_Velocity = Vector3.zero;

	[Header("Other")]
	public int CoyoteTime;
	private int CoyoteTimeFrames;

	private bool JumpAvailable;

	[SerializeField] private InputActionReference jumpAction;
	private bool jumpInputReleased;

	private Vector2 InputVector;

	private bool recharging_dash = false;

	[Header("Do Not Touch")]
	public bool dashing;
	public int dashes_remaining;
	public bool m_FacingRight = true;

	private int dashCooldownFrames = 0;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }


	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		grav = m_Rigidbody2D.gravityScale;

		dashing = false;
		dashes_remaining = MaxDashes;

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();
	}

	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		GetComponent<TrailRenderer>().emitting = dashing;
		m_Grounded = false;
		jumpInputReleased = !jumpAction.action.IsPressed();

		

		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
				

				if(m_DashOnCooldown == false && dashes_remaining < MaxDashes)
				{
					DashCooldown();
				}

				JumpAvailable = true;
				ResetCoyoteTime(CoyoteTime);
			}

		}

		if(recharging_dash && !m_Grounded)
		{
			recharging_dash = false;
		}

		if(CoyoteTimeFrames > 0)
		{
			CoyoteTimeFrames--;
		}

		if(dashCooldownFrames > 0 && m_DashOnCooldown)
		{
			if(m_Grounded && !dashing)
			{
				dashCooldownFrames--;
			}
			else
			{
				m_DashOnCooldown = false;
			}
		}

		else if(dashCooldownFrames <= 0 && m_DashOnCooldown)
		{
			recharging_dash = true;
			m_DashOnCooldown = false;
			StartCoroutine(DashRecharge());
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
			if (jump && JumpAvailable && !dashing)
			{
				m_Grounded = false;
				JumpAvailable = false;

				m_Rigidbody2D.linearVelocityY = 0;
				m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
				
					
			}
		}

		// auf y achse ausbremsen, wenn jump action released wurde
		if(jumpInputReleased && m_Rigidbody2D.linearVelocity.y > 0 && !dashing)
		{
			m_Rigidbody2D.linearVelocity = new Vector2(m_Rigidbody2D.linearVelocity.x, m_Rigidbody2D.linearVelocity.y / 1.5f);
		}
		else if(dashing)
		{
			m_Rigidbody2D.linearVelocity = new Vector2(m_Rigidbody2D.linearVelocity.x, m_Rigidbody2D.linearVelocity.y / DashYDamping);
		}
		

		if (dashes_remaining > 0 && dash)
		{
			if(m_Grounded && InputVector.y < 0.05)
			{
				InputVector.y = 0;
				print("test");
			}

			m_Grounded = false;
			dashes_remaining--;

			StartCoroutine(DashTime(0.13f));

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

	void DashCooldown()
	{
		m_DashOnCooldown = true;

		dashCooldownFrames = (int)Mathf.Ceil(m_DashCooldown * 50);
	}

	IEnumerator DashRecharge()
	{
		if(m_DashOnCooldown && !dashing)
		{
			RegainDash();
		}

		yield return new WaitForSeconds(m_DashRechargeTime);

		if(recharging_dash && dashes_remaining < MaxDashes && m_DashOnCooldown)
		{
			StartCoroutine(DashRecharge());
		}
		else
		{
			recharging_dash = false;
			m_DashOnCooldown = false;
		}
	}

	IEnumerator DashTime(float time)
	{
		m_Rigidbody2D.sharedMaterial = BouncyPlayerMat;
		StartCoroutine(DisableMoveForSec(time * 0.8f));
		m_Rigidbody2D.gravityScale = 0;
		dashing = true;

		yield return new WaitForSeconds(time);
		
		m_Rigidbody2D.sharedMaterial = PlayerMat;
		m_Rigidbody2D.gravityScale = grav;
		dashing = false;
	}

	public void RegainDash()
	{
		dashes_remaining++;
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
		m_Rigidbody2D.linearVelocityY = 0;
		m_Rigidbody2D.AddForce(new Vector2(0,m_PogoForce));
		RegainDash();
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