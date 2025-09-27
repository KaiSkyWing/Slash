using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Variables
    [SerializeField] private InputActionAsset inputActions;

    private InputAction m_moveAction;
    private InputAction m_jumpAction;

    private Vector2 m_moveAmt;
    private Rigidbody2D m_rigidbody;

    private const float jumpHeight = 20f;
    private const float walkSpeed = 10f;

    private bool isGrounded = false;
    private bool canAirJump = false;

    #endregion

    #region Start & Updates
    void Start()
    {
        m_moveAction = inputActions.FindAction("Move");
        m_jumpAction = inputActions.FindAction("Jump");

        m_moveAction.Enable();
        m_jumpAction.Enable();

        m_rigidbody = GetComponent<Rigidbody2D>();
        m_rigidbody.gravityScale = 3f;
    }

    void Update()
    {
        m_moveAmt = m_moveAction.ReadValue<Vector2>();

        if (m_jumpAction.WasPressedThisFrame())
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        Walking();
    }

    #endregion

    #region Custom Functions
    private void Jump()
    {
        if (isGrounded || canAirJump)
        {
            Vector2 velocity = m_rigidbody.velocity;
            velocity.y = 0;
            m_rigidbody.velocity = velocity;

            m_rigidbody.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);

            if (!isGrounded)
            {
                canAirJump = false;
            }
        }
    }

    private void Walking()
    {
        Vector2 velocity = m_rigidbody.velocity;
        velocity.x = m_moveAmt.x * walkSpeed;
        m_rigidbody.velocity = velocity;
    }

    #endregion

    #region Collisions
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    isGrounded = true;
                    canAirJump = true;
                    break;
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
    #endregion
}
