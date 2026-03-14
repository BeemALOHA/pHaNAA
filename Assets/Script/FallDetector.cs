using UnityEngine;

public class FallDetector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ตรวจสอบว่าสิ่งที่ตกลงมาชนคือ Player หรือไม่
        if (collision.CompareTag("Player"))
        {
            // ดึงคอมโพเนนต์ PlayerController ออกมา
            PlayerController player = collision.GetComponent<PlayerController>();

            if (player != null)
            {
                // ทางเลือกที่ 1: ให้ตายแล้วเกิดใหม่ (เลือดลดด้วย)
                player.TakeDamage(player.maxHealth);

                // หรือ ทางเลือกที่ 2: ถ้าอยากให้แค่ย้ายไปจุดเกิดใหม่เฉยๆ โดยเลือดไม่ลด
                // player.transform.position = player.respawnPoint.position;
            }
        }
    }
}
