using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelection : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 mouseStartPos;
    private Vector3 mouseEndPos;
    private bool isSelecting;
    private List<Unit> selectedUnits = new List<Unit>(); // เก็บยูนิตที่เลือก
    private List<ConstructionWorker> selectedWorkers = new List<ConstructionWorker>(); // เก็บ ConstructionWorker ที่เลือก

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        HandleSelection();
        HandleRightClickMove();
    }

    private void HandleSelection()
    {
        if (Input.GetMouseButtonDown(0)) // คลิกซ้ายเริ่มเลือก
        {
            mouseStartPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseStartPos.z = 0; // ให้ Z เป็น 0
            isSelecting = true;
            selectedUnits.Clear(); // ล้างยูนิตที่เลือกเมื่อเริ่มเลือกใหม่
            selectedWorkers.Clear(); // ล้าง ConstructionWorker ที่เลือก
        }

        if (Input.GetMouseButton(0)) // ขณะลากเมาส์
        {
            mouseEndPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseEndPos.z = 0; // ให้ Z เป็น 0
        }

        if (Input.GetMouseButtonUp(0)) // คลิกซ้ายปล่อย
        {
            SelectUnits();
            isSelecting = false;
        }
    }

    private void SelectUnits()
    {
        Vector3 min = Vector3.Min(mouseStartPos, mouseEndPos);
        Vector3 max = Vector3.Max(mouseStartPos, mouseEndPos);
        
        // ตรวจจับทุก collider ภายในพื้นที่ที่เลือก
        Collider2D[] hitColliders = Physics2D.OverlapAreaAll(min, max);
        foreach (var collider in hitColliders)
        {
            Unit unit = collider.GetComponent<Unit>();
            if (unit != null)
            {
                selectedUnits.Add(unit); // เพิ่ม Unit ที่เลือก
                continue; // ข้ามไปยูนิตถัดไป
            }

            ConstructionWorker worker = collider.GetComponent<ConstructionWorker>();
            if (worker != null)
            {
                selectedWorkers.Add(worker); // เพิ่ม ConstructionWorker ที่เลือก
            }
        }

        // Debug log ยูนิตที่ถูกเลือก
        Debug.Log("เลือก Unit: " + selectedUnits.Count + ", ConstructionWorker: " + selectedWorkers.Count);
    }

    private void HandleRightClickMove()
    {
        if (Input.GetMouseButtonDown(1)) // คลิกขวา
        {
            Vector3 targetPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = 0; // ให้ Z เป็น 0

            // ให้ยูนิตที่เลือกเดินไปยังตำแหน่งที่คลิก
            foreach (Unit unit in selectedUnits)
            {
                unit.SetTargetPosition(targetPosition);
            }

            // ให้ ConstructionWorker ที่เลือกเดินไปยังตำแหน่งที่คลิก
            foreach (ConstructionWorker worker in selectedWorkers)
            {
                worker.SetTargetPosition(targetPosition);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (isSelecting)
        {
            Vector3 topLeft = new Vector3(Mathf.Min(mouseStartPos.x, mouseEndPos.x), Mathf.Max(mouseStartPos.y, mouseEndPos.y), 0);
            Vector3 topRight = new Vector3(Mathf.Max(mouseStartPos.x, mouseEndPos.x), Mathf.Max(mouseStartPos.y, mouseEndPos.y), 0);
            Vector3 bottomLeft = new Vector3(Mathf.Min(mouseStartPos.x, mouseEndPos.x), Mathf.Min(mouseStartPos.y, mouseEndPos.y), 0);
            Vector3 bottomRight = new Vector3(Mathf.Max(mouseStartPos.x, mouseEndPos.x), Mathf.Min(mouseStartPos.y, mouseEndPos.y), 0);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
            Gizmos.DrawLine(bottomLeft, topLeft);
        }
    }
}
