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

    private float jumpForce = 10f;
    public float JumpForce { get => jumpForce; set => jumpForce = value; }
    private float acceleration = 10f;
    public float Acceleration { get => acceleration; set => acceleration = value; }
    private float maxVelocity = 5f;
    public float MaxVelocity { get => maxVelocity; set => maxVelocity = value; }
    private float direction;

    private float maxDrag = 5f;
    public float MaxDrag { get => maxDrag; set => maxDrag = value; }
    private readonly float minDrag = 0.25f;

    private readonly float coyoteTime = 0.25f;
    [SerializeField]
    private float currentCoyoteTime = 0f;

    private readonly float jumpBuffering = 0.25f;
    [SerializeField]
    private float currentJumpBuffer = 0f;

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
        while (currentJumpBuffer > 0f)
        {
            if (currentCoyoteTime > 0f)
            {
                print("HELL");
                currentCoyoteTime = 0f;
                rb.drag = minDrag;
                rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
                currentJumpBuffer = 0f;
            }
        yield return null;
        }
        yield return null;
    }

    private bool IsGrounded()
    {
        return rb.velocity.y == 0;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        //if(direction != 0)
        //{
        //    rb.velocity = new Vector2(direction * movementSpeed, rb.velocity.y);
        //}

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
