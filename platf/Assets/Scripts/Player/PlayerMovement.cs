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

    [SerializeField]
    private float jumpForce = 5f;
    [SerializeField]
    private float acceleration = 10f;
    [SerializeField]
    private float maxVelocity = 10f;

    private float direction;
    [SerializeField]
    private float maxDrag = 3f;
    [SerializeField]
    private float minDrag = 0.5f;

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
        moveAction.performed += Move;
        moveAction.canceled += CancelMove;

    }

    private void OnDisable()
    {
        jumpAction.performed -= Jump;
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
        print("Jump");
        TryToJump();
    }


    private void TryToJump()
    {
        if (IsGrounded())
        {
            rb.drag = minDrag;
            rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
        }
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

        if (!IsAtMaxVelocity())
            rb.AddForce(direction * acceleration * Vector2.right);

    }

    private bool IsAtMaxVelocity()
    {
        return rb.velocity.x >= maxVelocity || rb.velocity.x <= -maxVelocity;
    }
}
