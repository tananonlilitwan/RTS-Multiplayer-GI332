using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5f; // ความเร็วในการเคลื่อนที่ของกล้อง

    void Update()
    {
        // รับค่าการกดปุ่ม WASD
        float horizontal = Input.GetAxis("Horizontal"); // A/D หรือ Left/Right
        float vertical = Input.GetAxis("Vertical"); // W/S หรือ Up/Down

        // คำนวณการเคลื่อนที่ของกล้อง
        Vector3 movement = new Vector3(horizontal, vertical, 0f) * moveSpeed * Time.deltaTime;

        // เคลื่อนที่กล้อง
        transform.Translate(movement);
    }
}