using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
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
    public float restablishCooldown = 2f;
    [SerializeField] bool isFacingRight;
    [SerializeField] bool canAttack;
    [SerializeField] PlayerState currentState;

    [Header("GroundCheck Config")]
    [SerializeField] bool isGrounded;
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius;
    [SerializeField] LayerMask groundLayer;

    [Header("Knockback Configuration")]
    public float KnockbackX;
    public float KnockbackY;
    public float knockbackMultiplier = 1;
    Vector2 knockBackForce;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ( collision.gameObject.CompareTag("Attack") && currentState == PlayerState.normal )
        {
            //Trigger Animaciones 
            currentState = PlayerState.damaged;
            //Knockback segun posicion del que golpea
            if (collision.gameObject.transform.position.x < gameObject.transform.position.x)
            {
                //Knockback hacia el X positivo
                knockBackForce = new Vector2(KnockbackX, KnockbackY);
                rb.AddForce(knockBackForce * knockbackMultiplier);
                
            }
            else
            {
                knockBackForce = new Vector2(-KnockbackX,-KnockbackY);
                rb.AddForce(knockBackForce * knockbackMultiplier);
            }
            Invoke(nameof(ResetStatus), restablishCooldown);

        }
    }
    
    void ResetStatus()
    {
        currentState = PlayerState.normal;
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
        if (context.performed && isGrounded)
        {
            if (currentState == PlayerState.normal)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
            }
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed && currentState == PlayerState.normal)
        {
            if(canAttack)
            {
                anim.SetTrigger("Attack");
                canAttack = false;
                Invoke(nameof(ResetAttack), 2f);
            }
        }
    }
    void ResetAttack()
    {
        canAttack = true;
    }
}
