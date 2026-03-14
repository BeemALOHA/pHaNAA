using UnityEngine;

public class SlimeEnemy : MonoBehaviour
{
    public float attackRange = 1.5f;    // ระยะที่ Slime จะโจมตี
    public float attackRate = 2f;     // โจมตีทุกๆ กี่วินาที
    private float nextAttackTime = 0f;

    public float damage = 10f;        // พลังโจมตี
    public Transform player;          // ลาก Player มาใส่
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        // ถ้าไม่ได้ลาก Player มาใน Inspector ให้มันหาเองอัตโนมัติ
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        // คำนวณระยะห่างระหว่าง Slime กับ Player
        float distance = Vector2.Distance(transform.position, player.position);

        // ถ้าอยู่ในระยะ และ ถึงเวลาโจมตี
        if (distance <= attackRange && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackRate; // ตั้งเวลาการโจมตีครั้งต่อไป
        }
    }

    void Attack()
    {
        // สั่งเล่น Animation
        anim.SetTrigger("SlimeATK");

        // ส่งดาเมจไปที่ PlayerController
        PlayerController pc = player.GetComponent<PlayerController>();     
    }

    // วาดวงกลมในหน้า Scene เพื่อให้เราเห็นระยะโจมตี (ช่วยในการปรับค่า)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
    public void ApplyDamage()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= attackRange) // เช็คอีกรอบว่าตอนโดนตบ Player ยังอยู่ใกล้ไหม
        {
            player.GetComponent<PlayerController>().TakeDamage(damage);
        }
    }
    public void Die()
    {
        // เล่นแอนิเมชันตาย (ถ้ามี)
        anim.SetTrigger("SlimeDead");

        // ปิด Collider ของ Slime เพื่อไม่ให้เราไปโดนดาเมจซ้ำหลังจากมันตาย
        GetComponent<Collider2D>().enabled = false;

        // ทำลายวัตถุหลังจากเล่นแอนิเมชันเสร็จ (เช่น 0.5 วินาที)
        Destroy(gameObject, 0.5f);
    }
}
