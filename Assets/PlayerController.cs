using UnityEditor.Tilemaps;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed;
    public float jumpForce;
    private float inputHorizontal;
    private float inputVertical;
    private Rigidbody2D rb;
    private bool facingRight = true;

    [Header("jump")]
    public int extraJumpValue;
    public float jumpTime;
    private int extraJump;
    private float jumpTimeCounter;
    private bool isJumping;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask whatIsGround;
    private bool isGrounded;
    [Header("Effects")]
    public Animator camAnimator;
    private bool canPlayCamShake;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       rb = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

        inputHorizontal = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(inputHorizontal * speed, rb.linearVelocity.y);

        if (!facingRight && inputHorizontal > 0)
            Flip();
        else if (facingRight && inputHorizontal < 0)
            Flip();
    }

    void Update()
    {
        if (isGrounded == true)
        {
            if (canPlayCamShake)
            { 
                canPlayCamShake = false;
                camAnimator.SetTrigger("shake");              
            }
        }
        else
        
        {
            canPlayCamShake = true;
        }


        if (isGrounded)
        {
            extraJump = extraJumpValue;

            if (rb.linearVelocity.y < 0)
                isJumping = false;
        }

        if (Input.GetKeyDown(KeyCode.Space) && extraJump > 0)
        {
            isJumping = true;
            jumpTimeCounter = jumpTime;
            rb.linearVelocity = Vector2.up * jumpForce;
            extraJump--;
        }

        if (Input.GetKey(KeyCode.Space) && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                rb.linearVelocity = Vector2.up * jumpForce;
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
        }
    }
    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
    }
}

