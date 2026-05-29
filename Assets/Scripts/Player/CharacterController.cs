using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class CharacterController : MonoBehaviour
{
	[SerializeField] private float speed = 10f;
	[SerializeField] private float m_JumpForce = 1400f;
	[SerializeField] private float m_DashForce = 600f;
	[SerializeField] private float m_PogoForce = 600f;
	public float m_DashCooldown = 0.1f;

	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;
	[SerializeField] private bool m_AirControl = false;
	[SerializeField] private LayerMask m_WhatIsGround;
	[SerializeField] private Transform m_GroundCheck;

	const float k_GroundedRadius = .2f;
	private bool m_Grounded;
	private bool m_DashOnCooldown;
	private bool m_DashAvailable;
	private Rigidbody2D m_Rigidbody2D;
	public bool m_FacingRight = true;
	private Vector3 m_Velocity = Vector3.zero;

	[SerializeField] private InputActionReference jumpAction;
	private bool jumpInputReleased;


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
					print("Dash Available");
				}
			}
		}

		
	}


	public void Move(float move, bool jump, bool dash)
	{
		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{
			Vector3 targetVelocity;
			// Move the character by velocity
			if (!GetComponent<PlayerCombat>().light_dropped)
			{
				targetVelocity = new Vector2(move * speed, m_Rigidbody2D.linearVelocity.y);
			}
			else
			{
				targetVelocity = new Vector2(move * speed * GetComponent<BerserkMode>().speed_mult , m_Rigidbody2D.linearVelocity.y);
			}
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
		}
		// Player springt
		if (m_Grounded && jump)
		{
			m_Grounded = false;
			if (!GetComponent<PlayerCombat>().light_dropped)
			{
				m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
			}
			else
			{
				m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce * GetComponent<BerserkMode>().jump_force_mult));
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

			StartCoroutine(DashTimer());
			print("dash");

			float dir = 0;
			if (m_FacingRight)
			{
				dir = 1;
			}
			else
			{
				dir = -1;
			}

			if (!GetComponent<PlayerCombat>().light_dropped)
			{
				m_Rigidbody2D.AddForce(new Vector2(m_DashForce * dir, 0f));
			}
			else
			{
				m_Rigidbody2D.AddForce(new Vector2(m_DashForce * GetComponent<BerserkMode>().dash_force_mult * dir, 0f));
			}
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

	IEnumerator DashTimer()
	{
		m_DashOnCooldown = true;

		yield return new WaitForSeconds(m_DashCooldown);

		ResetDash();

	}

	public void ResetDash()
	{
		m_DashOnCooldown = false;
	}

	public void Pogo()
	{
		print("Pogo");
		m_Rigidbody2D.linearVelocityY = 0;
		m_Rigidbody2D.AddForce(new Vector2(0,m_PogoForce));
		ResetDash();
	}
}