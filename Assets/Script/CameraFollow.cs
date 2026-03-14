using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;        // ลาก Player มาใส่ในช่องนี้
    public float smoothSpeed = 0.125f; // ความเร็วในการตาม (ยิ่งน้อยยิ่งนุ่ม)
    public Vector3 offset = new Vector3(0, 0, -10); // ระยะห่าง (แกน Z ต้องเป็น -10 เสมอ)

    // ใช้ LateUpdate เพื่อให้กล้องขยับ "หลังจาก" ตัวละครขยับเสร็จแล้ว จะช่วยลดการสั่น
    void LateUpdate()
    {
        if (target == null) return;

        // ตำแหน่งที่กล้องอยากจะไป
        Vector3 desiredPosition = target.position + offset;

        // ใช้ Lerp เพื่อให้การเคลื่อนที่จากจุด A ไป B มีความนุ่มนวล
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // อัปเดตตำแหน่งกล้อง
        transform.position = smoothedPosition;
    }
}
