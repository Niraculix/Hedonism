using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class CharacterController : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 1400f;
	[SerializeField] private float m_DashForce = 600f;
	public float m_DashCooldown = 5f;

	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;
	[SerializeField] private bool m_AirControl = false;
	[SerializeField] private LayerMask m_WhatIsGround;
	[SerializeField] private Transform m_GroundCheck;

	const float k_GroundedRadius = .2f;
	private bool m_Grounded;
	private bool m_DashOnCooldown;
	private bool m_DashAvailable;
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;
	private Vector3 m_Velocity = Vector3.zero;

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
			// Move the character by velocity
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.linearVelocity.y);
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
		// If the player should jump...
		if (m_Grounded && jump)
		{
			// Add a vertical jumping-force to the player.
			m_Grounded = false;
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
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

			m_Rigidbody2D.AddForce(new Vector2(m_DashForce * dir, 0f));
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

		m_DashOnCooldown = false;

	}
}