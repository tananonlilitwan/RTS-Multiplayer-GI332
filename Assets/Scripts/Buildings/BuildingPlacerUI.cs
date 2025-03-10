using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*public class BuildingPlacerUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject buildingPrefab; // Prefab ของอาคารที่ต้องการสร้าง
    public Canvas canvas; // อ้างอิงถึง Canvas หลัก
    public RectTransform dragIcon; // ไอคอนที่ใช้แสดงตอนลากวาง
    private GameObject previewBuilding; // ตัวอย่างอาคารที่แสดงระหว่างลากวาง
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // สร้างไอคอนตัวอย่างอาคารเมื่อเริ่มลาก
        previewBuilding = Instantiate(buildingPrefab);
        previewBuilding.GetComponent<Collider2D>().enabled = false; // ปิดการชนระหว่างลาก
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (previewBuilding != null)
        {
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            previewBuilding.transform.position = mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (previewBuilding != null)
        {
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            previewBuilding.transform.position = mousePosition;
            previewBuilding.GetComponent<Collider2D>().enabled = true; // เปิดการชนเมื่อวาง
            previewBuilding = null;
        }
    }
}*/


public class BuildingPlacerUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject buildingPrefab; // Prefab ของอาคาร (ต้องตรงกับ objBuilding ใน BuildingManager)
    public Canvas canvas; // อ้างอิงถึง Canvas หลัก
    public RectTransform dragIcon; // ไอคอนที่ใช้แสดงตอนลากวาง
    private GameObject previewBuilding; // ตัวอย่างอาคารที่แสดงระหว่างลากวาง
    private Camera mainCamera;

    public BuildingManager buildingManager; // อ้างอิงถึง BuildingManager
    public int team = 0; // กำหนดทีมของผู้เล่น (เช่น 0 สำหรับผู้เล่น A)

    private void Start()
    {
        mainCamera = Camera.main;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // สร้างไอคอนตัวอย่างอาคารเมื่อเริ่มลาก
        previewBuilding = Instantiate(buildingPrefab);
        previewBuilding.GetComponent<Collider2D>().enabled = false; // ปิดการชนระหว่างลาก
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (previewBuilding != null)
        {
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            previewBuilding.transform.position = mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (previewBuilding != null)
        {
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            
            // เรียกใช้ BuildingManager เพื่อสร้างอาคารจริง
            buildingManager.ConstructBuilding(team, mousePosition);
            
            Destroy(previewBuilding); // ลบตัวอย่างอาคารเมื่อสร้างจริงแล้ว
        }
    }
}
