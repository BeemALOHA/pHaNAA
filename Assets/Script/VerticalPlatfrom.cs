using UnityEngine;

public class VerticalPlatform : MonoBehaviour
{
    private PlatformEffector2D effector2D;
    private float waitCout;
    public float waitTimeDefault = 0.3f; // ตั้งค่าเริ่มต้นไว้สัก 0.3 วินาที

    void Start()
    {
        effector2D = GetComponent<PlatformEffector2D>();
    }

    void Update()
    {
        // เมื่อปล่อยปุ่ม หรือกดกระโดด ให้รีเซ็ตแผ่นพื้นกลับมาปกติทันที
        if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.Space))
        {
            effector2D.rotationalOffset = 0f;
            waitCout = 0; // รีเซ็ตตัวนับ
        }

        // เมื่อกดปุ่มลงค้างไว้
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            if (waitCout <= 0)
            {
                // ถ้าตัวนับหมด ให้พลิกองศาพื้นเพื่อให้ตัวละครตกลงมา
                effector2D.rotationalOffset = 180f;
            }
            else
            {
                // ถ้ายกดค้างแต่เวลายังไม่หมด ให้ลบเวลาออกเรื่อยๆ
                waitCout -= Time.deltaTime;
            }
        }
        else
        {
            // ถ้าไม่ได้กดปุ่มลง ให้เตรียมเวลาไว้สำหรับครั้งต่อไปที่จะกด
            waitCout = waitTimeDefault;
        }
    }
}
