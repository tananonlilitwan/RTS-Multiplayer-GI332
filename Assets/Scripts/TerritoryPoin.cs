using System;
using UnityEngine;
using TMPro;

public class TerritoryPoint : MonoBehaviour
{
    public int currentOwner = -1; // -1 = ไม่มีทีมไหนยึด, 1 = ทีม A, 2 = ทีม B
    private int playerAUnits = 0; // จำนวนยูนิตของทีม A ที่อยู่ในพื้นที่
    private int playerBUnits = 0; // จำนวนยูนิตของทีม B ที่อยู่ในพื้นที่
    private float targetCapturePercentage = 0f; // เปอร์เซ็นต์เป้าหมายที่ต้องไปให้ถึงในการยึดพื้นที่
    private float currentCapturePercentage = 0f; // เปอร์เซ็นต์ปัจจุบันของการยึดพื้นที่

    public TMP_Text territoryStatusText; // ข้อความ UI ที่ใช้แสดงสถานะการยึดพื้นที่
    public float captureRadius = 5f; // รัศมีของพื้นที่ที่สามารถยึดได้
    private CircleCollider2D territoryCollider; // คอลลิเดอร์ที่ใช้ตรวจจับยูนิตในพื้นที่
    
    private LineRenderer lineRenderer; // เส้นวงกลมที่แสดงขอบเขตของพื้นที่ยึด
    public float baseCaptureSpeed = 5f; // ความเร็วพื้นฐานของการยึดพื้นที่

    private void Awake()
    {
        territoryCollider = gameObject.AddComponent<CircleCollider2D>();
        territoryCollider.radius = captureRadius;
        territoryCollider.isTrigger = true;

        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 50;
        lineRenderer.useWorldSpace = false;

        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        DrawCircle();
    }

    private void DrawCircle()
    {
        float angleStep = 360f / lineRenderer.positionCount;
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            float angle = i * angleStep;
            float x = Mathf.Cos(Mathf.Deg2Rad * angle) * captureRadius;
            float y = Mathf.Sin(Mathf.Deg2Rad * angle) * captureRadius;

            lineRenderer.SetPosition(i, new Vector3(x, y, 0f));
        }
    }
    
    private void UpdateTargetCapturePercentage()
    {
        if (playerAUnits > 0 && playerBUnits == 0)
        {
            targetCapturePercentage = 100f;
        }
        else if (playerBUnits > 0 && playerAUnits == 0)
        {
            targetCapturePercentage = 0f;
        }
    }
   
   private void OnTriggerEnter2D(Collider2D collision)
   {
       Unit unit = collision.GetComponent<Unit>();
       if (unit != null)
       {
           if (unit.team == 1)
           {
               if (currentOwner == -1) // ถ้ายังไม่มีใครยึดพื้นที่
               {
                   playerAUnits++;
               }
               else if (currentOwner == 2) // ถ้าทีม B เป็นเจ้าของ
               {
                   playerAUnits++;
               }
           }
           else if (unit.team == 2)
           {
               if (currentOwner == -1) // ถ้ายังไม่มีใครยึดพื้นที่
               {
                   playerBUnits++;
               }
               else if (currentOwner == 1) // ถ้าทีม A เป็นเจ้าของ
               {
                   playerBUnits++;
               }
           }
       }
       UpdateTargetCapturePercentage();
   }

   
   private void OnTriggerExit2D(Collider2D collision)
   {
       Unit unit = collision.GetComponent<Unit>();
       if (unit != null)
       {
           if (unit.team == 1)
           {
               playerAUnits = Mathf.Max(0, playerAUnits - 1);
           }
           else if (unit.team == 2)
           {
               playerBUnits = Mathf.Max(0, playerBUnits - 1);
           }
       }
       UpdateTargetCapturePercentage();
   }

   private void Update()
   {
       int totalUnits = playerAUnits + playerBUnits;
       float captureSpeed = baseCaptureSpeed + (totalUnits * 2f);
       
       // 🟢 team A กำลังยึด (และไม่มีทีม B อยู่)
       if (playerAUnits > 0 && playerBUnits == 0)
       {
           if (currentOwner == 2) // ถ้าเจ้าของปัจจุบันคือทีม B
           {
               // ลดเปอร์เซ็นต์จาก 100% → 0% ก่อน
               currentCapturePercentage = Mathf.MoveTowards(currentCapturePercentage, 0f, captureSpeed * Time.deltaTime);
               if (currentCapturePercentage <= 0f)
               {
                   currentOwner = -1; // รีเซ็ตเจ้าของพื้นที่ก่อน
               }
           }
           else
           {
               // เริ่มนับจาก 0% → 100% เพื่อให้ทีม A ยึด
               currentCapturePercentage = Mathf.MoveTowards(currentCapturePercentage, 100f, captureSpeed * Time.deltaTime);
               if (currentCapturePercentage >= 100f)
               {
                   currentOwner = 1;
                   Debug.Log($"Territory changed owner! New owner: {currentOwner}");
               }
           }
       }
       
       // 🔴 Team B กำลังยึด (และไม่มีทีม A อยู่)
       else if (playerBUnits > 0 && playerAUnits == 0)
       {
           if (currentOwner == 1) // ถ้าเจ้าของปัจจุบันคือทีม A
           {
               // ลดเปอร์เซ็นต์จาก 100% → 0% ก่อน
               currentCapturePercentage = Mathf.MoveTowards(currentCapturePercentage, 0f, captureSpeed * Time.deltaTime);
               if (currentCapturePercentage <= 0f)
               {
                   currentOwner = -1; // รีเซ็ตเจ้าของพื้นที่ก่อน
               }
           }
           else
           {
               // เริ่มนับจาก 0% → 100% เพื่อให้ทีม B ยึด
               currentCapturePercentage = Mathf.MoveTowards(currentCapturePercentage, 100f, captureSpeed * Time.deltaTime);
               if (currentCapturePercentage >= 100f)
               {
                   currentOwner = 2;
                   Debug.Log($"Territory changed owner! New owner: {currentOwner}");
               }
           }
       }
       
       // อัปเดตข้อความแสดงเปอร์เซ็นต์การยึดครอง
       if (territoryStatusText != null)
       {
           string ownerText = currentOwner switch
           {
               1 => "Team A",
               2 => "Team B",
               _ => "No Owner"
           };
           //territoryStatusText.text = $"Capture: {Mathf.Round(currentCapturePercentage)}%";
           territoryStatusText.text = $"Capture: {Mathf.Round(currentCapturePercentage)}%\nOwner: {ownerText}";
       }
   }
   
   
}
