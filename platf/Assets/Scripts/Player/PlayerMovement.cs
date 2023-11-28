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

    private bool isGrounded = true;

    private float jumpForce = 5f;
    private float movementSpeed = 10f;
    
    private float direction;
    [SerializeField]
    private float drag = 10;

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
        //jumpAction.performed += Jump;
        //jumpAction.canceled += Jump;

    }

    private void OnDisable()
    {
        jumpAction.performed -= Jump;
        moveAction.performed -= Move;
        moveAction.canceled -= CancelMove;
        //jumpAction.performed -= Jump;
        //jumpAction.canceled -= Jump;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Move(InputAction.CallbackContext context)
    {
        direction = context.ReadValue<float>();
        rb.drag = 0;
    }

    private void CancelMove(InputAction.CallbackContext context)
    {
        direction = 0;
        rb.drag = drag;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        print("Jump");
        TryToJump(isGrounded);

    }

    private void TryToJump(bool isPlayerGrounded)
    {
        if (isPlayerGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if(direction != 0)
            rb.velocity = direction * movementSpeed * Vector2.right;
    }
}
