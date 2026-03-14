using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // ตัวแปร static เพื่อเก็บตัวตนของ SoundManager ตัวแรกที่เกิดขึ้น
    private static SoundManager instance;

    void Awake()
    {
        // ระบบ Singleton: ตรวจสอบว่ามี SoundManager อยู่ในเกมหรือยัง
        if (instance == null)
        {
            // ถ้ายังไม่มี ให้ตัวนี้เป็นตัวหลัก
            instance = this;
            // สั่งให้ "ไม่ต้องลบวัตถุนี้" เมื่อเปลี่ยน Scene
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // ถ้ามีตัวเดิมอยู่แล้ว (เช่น กลับมาจากหน้าเกมเข้าหน้าเมนู)
            // ให้ลบตัวใหม่ทิ้งซะ เพื่อไม่ให้เพลงดังซ้อนกัน 2 เพลง
            Destroy(gameObject);
        }
    }
}
