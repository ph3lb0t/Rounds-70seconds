using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; //Add Input System Lib

public class PlayerControllerMulti : MonoBehaviour
{
    //private refs
    Animator anim;
    Rigidbody2D rb;
    PlayerInput playerInput; //Para leer las nuevas inputs
    Vector2 horInput;
    
    public enum PlayerState { normal, damaged }

    [Header("Character Stats & Status")]
    public float speed;
    public float jumpForce;
    [SerializeField] bool isFacingRight;
    [SerializeField] bool canAttack;
    [SerializeField] PlayerState currentState;

    [Header("GroundCheck Config")]
    [SerializeField] bool isGrounded;
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius;
    [SerializeField] LayerMask groundLayer;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        currentState = PlayerState.normal;
        canAttack = true;
        isFacingRight = true;
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        //Detector continuo de Flip
        FlipUpdater();
        
        if (currentState == PlayerState.normal)
        {
            horInput = playerInput.actions["Movement"].ReadValue<Vector2>();
        }
    }

    private void FixedUpdate()
    {
        if (currentState == PlayerState.normal) { Movement(); }
    }

    void Movement()
    {
        rb.velocity = new Vector2(horInput.x * speed, rb.velocity.y);
    }

    void Flip()
    {
        Vector3 currentSacle = transform.localScale;
        currentSacle.x += -1;
        transform.localScale = currentSacle;
        isFacingRight = !isFacingRight;

    }

    void FlipUpdater()
    {
        if ( horInput.x > 0 )
        {
            if (!isFacingRight)
            {
                Flip();
            }
        }
        if(horInput.x < 0)
        {
            if (isFacingRight)
            {
                Flip();
            }
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {

    }
}
