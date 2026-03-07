using UnityEditor.Tilemaps;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed;
    public float jumpForce;
    private float inputHorizontal;
    private float inputVertical;
    private Rigidbody2D rb;
    private bool facingRight = true;
    [Header("Jump")]
    public int extraJumpsValue;
    public float jumpTime;
    private int extraJumps;
    private float jumpTimeCounter;
    private bool isJumping;
    [Header("Ground")]
    private bool isGrounded;
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask whatIsGround;
    [Header("Ladder")]
    public float distance;
    public LayerMask whatIsLadder;
    private bool isClimbing;
    RaycastHit2D hitInfo;
    [Header("Effect")]
    public GameObject dustEffect;
    private bool spawnDust;
    public Animator camAnimator;
    public Animator playerAnimator;

    void Start()
    {
        extraJumps = extraJumpsValue;
        rb = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius,whatIsGround);
        inputHorizontal = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(inputHorizontal * speed, rb.linearVelocity.y);
        if(facingRight == false && inputHorizontal > 0)
        {
            Flip();
        }
        else if(facingRight == true && inputHorizontal < 0)
        {
            Flip();
        }
    }
    void Update()
    {
        PlayerInput();

        playerAnimator.SetFloat("speed", Mathf.Abs(rb.linearVelocity.x));
        playerAnimator.SetBool("jump", !isGrounded);

        if (isGrounded == true)
        {
            if(spawnDust == true)
            {
                camAnimator.SetTrigger("shake");
                Instantiate(dustEffect, groundCheck.position, Quaternion.identity);
                spawnDust = false;
            }
        }
        else
        {
            spawnDust = true;
        }

        if (isGrounded == true)
        {
            extraJumps = extraJumpsValue;
            if (rb.linearVelocity.y == 0)
            {
                isJumping = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && extraJumps > 0)
        {
            Instantiate(dustEffect, groundCheck.position, Quaternion.identity);
            isJumping = true;
            jumpTimeCounter = jumpTime;
            rb.linearVelocity = Vector2.up * jumpForce;
            extraJumps--;
        }
        else if(Input.GetKeyDown(KeyCode.Space) && extraJumps == 0 && isGrounded == true)
        {
            Instantiate(dustEffect, groundCheck.position, Quaternion.identity);
            isJumping = true;
            jumpTimeCounter = jumpTime;
            rb.linearVelocity = Vector2.up * jumpForce;
        }

        if (Input.GetKey(KeyCode.Space) && isJumping)
        {
            if(jumpTimeCounter > 0)
            {
                rb.linearVelocity = Vector2.up * jumpForce;
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }
        if(Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
        }

        hitInfo = Physics2D.Raycast(transform.position, Vector2.up, distance, whatIsLadder);
        if (hitInfo.collider != null)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                isClimbing = true;
            }
        }
        else
        {
            isClimbing = false;
        }

        if (isClimbing == true && hitInfo.collider != null)
        {
            inputVertical = Input.GetAxisRaw("Vertical");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, inputVertical * speed);
            rb.gravityScale = 0;
        }
        else
        {
            rb.gravityScale = 5;
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }
    void PlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            playerAnimator.SetTrigger("victory");
        }
    }
}

