using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinValue = 1; // เหรียญนี้มีค่าเท่าไหร่
    public GameObject collectEffect; // (Optional) เอฟเฟกต์ตอนเก็บเหรียญ

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ตรวจสอบว่าสิ่งที่มาชนคือ Player หรือไม่
        if (collision.CompareTag("Player"))
        {
            // เรียกฟังก์ชันเพิ่มคะแนนจาก PlayerController
            PlayerController player = collision.GetComponent<PlayerController>();

            if (player != null)
            {
                player.AddScore(coinValue);

                // สร้างเอฟเฟกต์ (ถ้ามี)
                if (collectEffect != null)
                {
                    Instantiate(collectEffect, transform.position, Quaternion.identity);
                }

                // ทำลายเหรียญทิ้ง
                Destroy(gameObject);
            }
        }
    }
}
