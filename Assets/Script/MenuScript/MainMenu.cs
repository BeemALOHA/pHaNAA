using UnityEngine;
using UnityEngine.SceneManagement; // จำเป็นต้องมีบรรทัดนี้เพื่อเปลี่ยนฉาก

public class MainMenu : MonoBehaviour
{
    // ฟังก์ชันสำหรับเริ่มเกม (กดปุ่ม Start)
    public void StartGame()
    {
        // โหลดฉากที่ชื่อว่า level1
        SceneManager.LoadScene("level1");
    }

    // ฟังก์ชันสำหรับไปหน้า Credit
    public void Credit()
    {
        // โหลดฉากที่ชื่อว่า Credit (ระวังพิมพ์ผิดเป็น Crebit ตามในชื่อไฟล์จริงนะครับ)
        SceneManager.LoadScene("Credit");
    }

    // ฟังก์ชันสำหรับกลับหน้าเมนูหลัก (ถ้าต้องการใช้ในหน้าอื่น)
    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    // ฟังก์ชันออกจากเกม
    public void QuitGame()
    {
        Debug.Log("Game is exiting...");
        Application.Quit();
    }
}