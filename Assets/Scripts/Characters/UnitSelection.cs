using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSelection : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 mouseStartPos;
    private Vector3 mouseEndPos;
    private bool isSelecting;
    private List<Unit> selectedUnits = new List<Unit>(); // เก็บยูนิตที่เลือก
    private List<ConstructionWorker> selectedWorkers = new List<ConstructionWorker>(); // เก็บ ConstructionWorker ที่เลือก
    
    public RectTransform selectionBoxUI; // อ้างถึง Image (RectTransform)
    private Vector2 startMousePos;
   
   

    private void Start()
    {
        mainCamera = Camera.main;
        selectionBoxUI.gameObject.SetActive(false); // ปิดตอนเริ่ม
    }

    private void Update()
    {
        HandleSelection();
        HandleRightClickMove();
        
        // ป้องกัน UI ค้างถ้าปล่อยเมาส์แต่ isSelecting ยังเป็น true
        if (!Input.GetMouseButton(0) && isSelecting)
        {
            Debug.Log("Force clear UI - fallback");
            isSelecting = false;
            selectionBoxUI.gameObject.SetActive(false);
            selectionBoxUI.sizeDelta = Vector2.zero;
            selectionBoxUI.anchoredPosition = Vector2.zero;
        }
    }

    private void HandleSelection()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            //Debug.Log("เมาส์อยู่บน UI, ข้าม HandleSelection");
            return;
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse Down");

            mouseStartPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseStartPos.z = 0;
            startMousePos = Input.mousePosition;

            isSelecting = true;
            selectedUnits.Clear();
            selectedWorkers.Clear();
            selectionBoxUI.gameObject.SetActive(true);
        }

        if (Input.GetMouseButton(0))
        {
            mouseEndPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseEndPos.z = 0;

            Vector2 currentMousePos = Input.mousePosition;
            UpdateSelectionBox(startMousePos, currentMousePos);
        }

        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Mouse Up");

            mouseEndPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseEndPos.z = 0;

            UpdateSelectionBox(startMousePos, Input.mousePosition);
            Canvas.ForceUpdateCanvases();

            selectionBoxUI.sizeDelta = Vector2.zero;
            selectionBoxUI.anchoredPosition = Vector2.zero;
            selectionBoxUI.gameObject.SetActive(false);

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

    private void UpdateSelectionBox(Vector2 screenStart, Vector2 screenEnd)
    {
        Vector2 startLocalPos;
        Vector2 endLocalPos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            selectionBoxUI.parent as RectTransform,
            screenStart,
            null, // ถ้า Canvas เป็น Overlay ให้ใช้ null, ถ้าเป็น Camera ให้ใส่ mainCamera
            out startLocalPos
        );

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            selectionBoxUI.parent as RectTransform,
            screenEnd,
            null,
            out endLocalPos
        );

        Vector2 size = endLocalPos - startLocalPos;
        selectionBoxUI.anchoredPosition = startLocalPos + size / 2;
        selectionBoxUI.sizeDelta = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));
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