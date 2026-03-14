using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WarpToMenu : MonoBehaviour
{
    [Header("Settings")]
    public string menuSceneName = "Menu"; // ใส่ชื่อฉากเมนูของคุณที่นี่

    // ฟังก์ชันนี้จะทำงานเมื่อมีวัตถุที่มี Collider เข้ามาสัมผัส
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ตรวจสอบว่าสิ่งที่มาชนคือ Player หรือไม่
        if (collision.CompareTag("Player"))
        {
            // ทำความสะอาดค่า TimeScale (เผื่อมีการหยุดเกมไว้)
            Time.timeScale = 1f;

            // วาร์ปกลับหน้าเมนู
            SceneManager.LoadScene("Menu");
        }
    }
}
