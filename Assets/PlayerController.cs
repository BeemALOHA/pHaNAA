using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed;
    public float jumpForce;
    private float inputHorizontal;
    private float inputVertical;
    private Rigidbody2D rb;
    private bool facingRight = true;

    [Header("Jump & Stamina")]
    public int extraJumpsValue;
    public float jumpTime;
    private int extraJumps;
    private float jumpTimeCounter;
    private bool isJumping;
    public float jumpStaminaCost = 20f; 

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

    [Header("Effect & Animation")]
    public GameObject dustEffect;
    private bool spawnDust;
    public Animator camAnimator;
    public Animator playerAnimator;

    [Header("Health System")]
    public float maxHealth = 100f;
    public float currentHealth;
    public Slider healthSlider; 

    [Header("Stamina System")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenRate = 15f; 
    public Slider staminaSlider;

    [Header("Respawn System")]
    public Transform respawnPoint; // ลากจุดเกิดมาใส่ที่นี่ใน Inspector

    [Header("Score System")]
    public int score = 0;
    public TextMeshProUGUI scoreText; // 2. เปลี่ยนจาก Text เป็น TextMeshProUGUI

    [Header("Invincibility")]
    public float invincibilityDuration = 2f; // ระยะเวลาอมตะ
    private bool isInvincible = false;       // เช็คว่าตอนนี้อมตะอยู่ไหม
    public SpriteRenderer playerSprite;      // ลาก Sprite ของตัวละครมาใส่เพื่อให้กะพริบได้

    [Header("Stomp Settings")]
    public float bounceForce = 15f; // แรงเด้งตัวเมื่อเหยียบหัวศัตรู

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        extraJumps = extraJumpsValue;
        
        // ตั้งค่าเริ่มต้น HP และ Stamina
        currentHealth = maxHealth;
        currentStamina = maxStamina;

        if (healthSlider != null) healthSlider.maxValue = maxHealth;
        if (staminaSlider != null) staminaSlider.maxValue = maxStamina;
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
        playerAnimator.SetBool("jump", !isGrounded);

        inputHorizontal = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(inputHorizontal * speed, rb.linearVelocity.y);

        // Animation Running
        playerAnimator.SetBool("Running", inputHorizontal != 0);

        if (facingRight == false && inputHorizontal > 0) Flip();
        else if (facingRight == true && inputHorizontal < 0) Flip();
    }

    void Update()
    {
        UpdateUI();
        RegenStamina();
        PlayerInput();     

        // ระบบตกถึงพื้น (Land Effect)
        if (isGrounded)
        {
            if (spawnDust)
            {
                camAnimator.SetTrigger("shake");
                playerAnimator.SetTrigger("Ground");
                Instantiate(dustEffect, groundCheck.position, Quaternion.identity);
                spawnDust = false;
            }
            extraJumps = extraJumpsValue;
            if (rb.linearVelocity.y <= 0) isJumping = false;
        }
        else
        {
            spawnDust = true;
        }

        // --- ระบบกระโดดแบบใช้ Stamina ---
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // ตรวจสอบว่ามี Stamina พอไหม และ (อยู่บนพื้น หรือ มีกระโดดเสริมเหลืออยู่)
            if (currentStamina >= jumpStaminaCost && (isGrounded || extraJumps > 0))
            {
                PerformJump();
                if (!isGrounded) extraJumps--; 
                
                currentStamina -= jumpStaminaCost; // ลด Stamina
            }
        }

        // การกดค้างเพื่อกระโดดสูง (Variable Jump Height)
        if (Input.GetKey(KeyCode.Space) && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                rb.linearVelocity = Vector2.up * jumpForce;
                jumpTimeCounter -= Time.deltaTime;
            }
            else { isJumping = false; }
        }

        if (Input.GetKeyUp(KeyCode.Space)) isJumping = false;

        // ระบบบันได (Ladder)
        HandleLadder();
    }

    void PerformJump()
    {
        Instantiate(dustEffect, groundCheck.position, Quaternion.identity);
        isJumping = true;
        jumpTimeCounter = jumpTime;
        rb.linearVelocity = Vector2.up * jumpForce;
    }

    void HandleLadder()
    {
        hitInfo = Physics2D.Raycast(transform.position, Vector2.up, distance, whatIsLadder);
        if (hitInfo.collider != null)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) isClimbing = true;
        }
        else { isClimbing = false; }

        if (isClimbing && hitInfo.collider != null)
        {
            inputVertical = Input.GetAxisRaw("Vertical");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, inputVertical * speed);
            rb.gravityScale = 0;
        }
        else { rb.gravityScale = 5; }
    }

    // --- ระบบเลือดและ UI ---
    public void TakeDamage(float damage)
    {
        // 1. ถ้าตายอยู่ หรือ อยู่ในสถานะอมตะ ให้หยุดฟังก์ชันทันที (ไม่โดนดาเมจ)
        if (currentHealth <= 0 || isInvincible) return;

        currentHealth -= damage;
        playerAnimator.SetTrigger("hurt");

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            StartCoroutine(DeathRoutine());
        }
        else
        {
            // 2. ถ้ายังไม่ตาย ให้เริ่มสถานะอมตะ
            StartCoroutine(BecomeInvincible());
        }
    }

    // ฟังก์ชันลำดับการตายและการเกิดใหม่
    System.Collections.IEnumerator DeathRoutine()
    {
        playerAnimator.SetTrigger("Dead");

        // หยุดการเคลื่อนที่ของตัวละครขณะตาย
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false; // ปิด Physics ชั่วคราวไม่ให้ตัวละครขยับหรือตกฉาก

        yield return new WaitForSeconds(2f); // รอ 2 วินาทีก่อนเกิดใหม่ (ปรับเวลาได้)

        Respawn();
    }

    void UpdateUI()
    {
        if (healthSlider != null) healthSlider.value = currentHealth;
        if (staminaSlider != null) staminaSlider.value = currentStamina;
    }

    void RegenStamina()
    {
        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
        }
    }

    // ตรวจสอบการชนกับศัตรู (ตัวอย่าง)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(10f);
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
        if (Input.GetKeyDown(KeyCode.Alpha1)) playerAnimator.SetTrigger("victory");
        if (Input.GetKeyDown(KeyCode.Alpha2)) TakeDamage(10); // ทดสอบลดเลือด
        if (Input.GetKeyDown(KeyCode.Alpha3)) playerAnimator.SetTrigger("Dead");
        if (Input.GetKeyDown(KeyCode.Alpha4)) playerAnimator.SetTrigger("Spawn");
    }
    void Respawn()
    {
        // ย้ายตำแหน่งตัวละครไปยังจุด Respawn
        transform.position = respawnPoint.position;

        // รีเซ็ตค่าต่างๆ
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        rb.simulated = true; // เปิด Physics อีกครั้ง

        // เล่นแอนิเมชันตอนเกิด (ถ้ามี)
        playerAnimator.SetTrigger("Spawn");

        // ปรับหน้าให้หันไปทางขวา (Optional)
        if (!facingRight) Flip();
    }
    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "" + score.ToString();
        }
    }
    private IEnumerator BecomeInvincible()
    {
        isInvincible = true;

        // วนลูปการกะพริบตัวละคร
        float timer = 0;
        while (timer < invincibilityDuration)
        {
            // สลับความโปร่งใส (กะพริบ)
            playerSprite.color = new Color(1f, 1f, 1f, 0.5f); // จางลง
            yield return new WaitForSeconds(0.1f);
            playerSprite.color = new Color(1f, 1f, 1f, 1f);   // กลับมาปกติ
            yield return new WaitForSeconds(0.1f);

            timer += 0.2f;
        }

        isInvincible = false; // หมดช่วงอมตะ
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ตรวจสอบว่าชนกับจุดอ่อน (HeadCheck) หรือไม่
        if (collision.CompareTag("WeakPoint"))
        {
            // 1. สั่งให้ Slime ตาย
            // ดึง Script จากตัวพ่อ (เพราะ HeadCheck เป็นลูกของ Slime)
            SlimeEnemy slime = collision.GetComponentInParent<SlimeEnemy>();
            if (slime != null)
            {
                slime.Die();

                // 2. ทำให้ตัวละครเด้งขึ้น
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceForce);

                // 3. (Optional) เพิ่มคะแนน
                AddScore(50);
            }
        }
    }
}
