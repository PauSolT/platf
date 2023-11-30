using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction jumpAction;
    private InputAction moveAction;
    private Rigidbody2D rb;

    private float jumpForce = 17f;
    public float JumpForce { get => jumpForce; set => jumpForce = value; }
    private float acceleration = 20f;
    public float Acceleration { get => acceleration; set => acceleration = value; }
    private float maxVelocity = 8f;
    public float MaxVelocity { get => maxVelocity; set => maxVelocity = value; }
    private float direction;

    private float maxDrag = 5f;
    public float MaxDrag { get => maxDrag; set => maxDrag = value; }
    private readonly float minDrag = 0.25f;

    private readonly float coyoteTime = 0.2f;
    private float currentCoyoteTime = 0f;

    private readonly float jumpBuffering = 0.15f;
    private float currentJumpBuffer = 0f;

    private readonly int layerMasksGround = 1 << 3 | 1 << 6 | 1 << 7 | 1 << 8;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        jumpAction = playerInput.actions["Jump"];
        moveAction = playerInput.actions["Move"];
    }

    private void OnEnable()
    {
        jumpAction.performed += Jump;
        jumpAction.canceled += CancelJump;
        moveAction.performed += Move;
        moveAction.canceled += CancelMove;
    }

    private void OnDisable()
    {
        jumpAction.performed -= Jump;
        jumpAction.canceled -= CancelJump;
        moveAction.performed -= Move;
        moveAction.canceled -= CancelMove;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Move(InputAction.CallbackContext context)
    {
        direction = context.ReadValue<float>();
        rb.drag = minDrag;
    }

    private void CancelMove(InputAction.CallbackContext context)
    {
        direction = 0;
        if (IsGrounded())
        {
            rb.drag = maxDrag;
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        currentJumpBuffer = jumpBuffering;
        StartCoroutine(nameof(TryToJump));
    }

    private void CancelJump(InputAction.CallbackContext context)
    {
        StartCoroutine(nameof(TickCurrentJumpBuffer));
    }


    private IEnumerator TickCurrentJumpBuffer()
    {   
        while (currentJumpBuffer >0f)
        {
            currentJumpBuffer -= Time.fixedDeltaTime;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        yield return null;
    }


    private IEnumerator TryToJump()
    {
        while (currentJumpBuffer > 0f )
        {
            if (currentCoyoteTime > 0f && IsGrounded())
            {
                currentCoyoteTime = 0f;
                rb.drag = minDrag;
                rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
                currentJumpBuffer = 0f;

                // Break out of the coroutine to prevent double jumps
                yield break;
            }

            yield return null;
        }
    }

    private bool IsGrounded()
    {
        Ray ray = new(transform.position, -transform.up);

        RaycastHit2D hit = Physics2D.Raycast(rb.position, Vector2.down, 0.46f, layerMasksGround);
        if (hit)
            print(hit.collider.name);

        Debug.DrawRay(ray.origin, ray.direction * 0.46f, Color.green);


        return hit.collider != null;
    }

    void FixedUpdate()
    {
        if (IsGrounded())
        {
            currentCoyoteTime = coyoteTime;
        } else
        {
            currentCoyoteTime -= Time.deltaTime;
        }



        if (!IsAtMaxVelocity())
            rb.AddForce(direction * acceleration * Vector2.right);

    }

    private bool IsAtMaxVelocity()
    {
        return rb.velocity.x >= maxVelocity || rb.velocity.x <= -maxVelocity;
    }
}
