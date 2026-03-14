using UnityEngine;

public class SlimeBoss : MonoBehaviour
{
    [Header("Stats")]
    public int timesToHit = 10;      // ต้องเหยียบ 10 ครั้งถึงตาย
    private int currentHits = 0;

    [Header("Movement (Jump Pattern)")]
    public float jumpForceX = 5f;
    public float jumpForceY = 8f;
    public float idleTime = 1.5f;
    private float nextJumpTime;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float checkRadius = 0.3f;
    public LayerMask whatIsGround;
    private bool isGrounded;

    private Rigidbody2D rb;
    private Transform player;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

        // ระบบกระโดดวนลูป (Pattern ง่ายๆ)
        if (isGrounded && Time.time >= nextJumpTime)
        {
            JumpToPlayer();
            nextJumpTime = Time.time + idleTime;
        }

        // ส่งค่าเข้า Animator (ถ้ามี parameter ชื่อ isGrounded)
        if (anim != null) anim.SetBool("isGrounded", isGrounded);
    }

    void JumpToPlayer()
    {
        if (player == null) return;

        // สั่งเล่นท่ากระโดด
        if (anim != null) anim.SetTrigger("Jump");

        // หาทางไปหา Player
        float direction = (player.position.x > transform.position.x) ? 1 : -1;

        // กลับหน้า Sprite
        transform.localScale = new Vector3(-direction * Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);

        // กระโดด!
        rb.linearVelocity = new Vector2(direction * jumpForceX, jumpForceY);
    }

    // ฟังก์ชันนี้จะถูกเรียกจาก PlayerController เมื่อเหยียบหัว
    public void GotStomped()
    {
        currentHits++;

        // เล่น Effect หรือเสียงตอนโดนเหยียบ
        if (anim != null) anim.SetTrigger("hurt");

        Debug.Log("Boss HP: " + (timesToHit - currentHits));

        if (currentHits >= timesToHit)
        {
            Die();
        }
    }

    void Die()
    {
        if (anim != null) anim.SetTrigger("SlimeDead");

        // ปิดการทำงานทุกอย่าง
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
        rb.simulated = false;

        Destroy(gameObject, 1f); // รอให้เล่นท่าตายจบก่อนลบ
    }
}
