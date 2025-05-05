using UnityEngine;
using TMPro;
using UnityEngine.UI; // เพิ่มการใช้งาน UI

public class Base : MonoBehaviour
{
    public int team; // 0 สำหรับผู้เล่น A, 1 สำหรับผู้เล่น B
    public int health = 1000; // ค่าเริ่มต้นของพลังชีวิตฐานคือ 1000
    public TextMeshProUGUI healthTextTMP; // ถ้าใช้ TextMeshPro

    public GameObject unitPrefab; // Prefab ของ Unit
    public GameObject constructionWorkerPrefab; // Prefab ของ ConstructionWorker
    public Transform spawnPoint; // จุดเกิดยูนิต (ตั้งค่าจาก Inspector)

    public float unitCost = 50f; // ค่าใช้จ่ายในการสร้างยูนิต

    public Button buyUnitButton; // ปุ่มสำหรับซื้อยูนิต
    public Button buyConstructionWorkerButton; // ปุ่มสำหรับซื้อ Construction Worker

    public GameObject buildingPrefab; // ต้นแบบอาคาร
    public GameObject buildingPreview; // ตัวอย่างอาคารก่อนวาง
    private GameObject previewInstance;
    private bool isPlacingBuilding = false;
    
    private void Start()
    {
        UpdateHealthUI(); // เรียกใช้ตอนเริ่มเกมเพื่อแสดงค่า HP ตอนเริ่ม

        // ผูกปุ่ม UI กับฟังก์ชัน
        buyUnitButton.onClick.AddListener(OnBuyUnitButtonClicked);
        buyConstructionWorkerButton.onClick.AddListener(OnBuyConstructionWorkerButtonClicked);
    }

    private void Update()
    {
        // เช็คคลิกขวา
        if (Input.GetMouseButtonDown(1)) // 1 คือปุ่มขวาของเมาส์
        {
            // คำสั่งที่จะถูกเรียกเมื่อคลิกขวา (หรือให้สามารถคลิกปุ่มได้)
            TryRightClickToBuyUnit();
        }
        
        // เช็คคลิกขวา
        if (Input.GetMouseButtonDown(1)) // 1 คือปุ่มขวาของเมาส์
        {
            // คำสั่งที่จะถูกเรียกเมื่อคลิกขวา (หรือให้สามารถคลิกปุ่มได้)
            if (isPlacingBuilding) // ถ้ากำลังวางอาคาร
            {
                CancelPlacingBuilding(); // ยกเลิกการวางอาคาร
            }
            else
            {
                TryRightClickToBuyUnit(); // การซื้อยูนิต (กรณีคลิกขวาบน UI)
            }
        }

        if (isPlacingBuilding && previewInstance != null)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            previewInstance.transform.position = mousePos;

            if (Input.GetMouseButtonDown(0)) // คลิกเพื่อวางอาคาร
            {
                PlaceBuilding(mousePos);
            }
        }
    }

    // ฟังก์ชันที่จะถูกเรียกเมื่อคลิกขวาที่ UI สำหรับการซื้อยูนิต
    private void TryRightClickToBuyUnit()
    {
        // คำสั่งที่นี่คือการตรวจสอบว่าผู้เล่นคลิกขวาที่ UI หรือไม่
        if (RectTransformUtility.RectangleContainsScreenPoint(buyUnitButton.GetComponent<RectTransform>(), Input.mousePosition))
        {
            OnBuyUnitButtonClicked(); // คลิกซื้อยูนิต
        }
        else if (RectTransformUtility.RectangleContainsScreenPoint(buyConstructionWorkerButton.GetComponent<RectTransform>(), Input.mousePosition))
        {
            OnBuyConstructionWorkerButtonClicked(); // คลิกซื้อ Construction Worker
        }
    }

    // ฟังก์ชันที่จะถูกเรียกเมื่อคลิกปุ่มซื้อยูนิต
    private void OnBuyUnitButtonClicked()
    {
        BuyUnit(spawnPoint.position);
    }

    // ฟังก์ชันที่จะถูกเรียกเมื่อคลิกปุ่มซื้อ Construction Worker
    private void OnBuyConstructionWorkerButtonClicked()
    {
        SpawnConstructionWorker();
    }

    public void BuyUnit(Vector3 spawnPosition)
    {
        if (ResourceManager.instance.SpendResources(team, (int)unitCost))         //if (ResourceManager.instance.SpendWood(unitCost))
        {
            Instantiate(unitPrefab, spawnPosition, Quaternion.identity);
            //Debug.Log("ซื้อยูนิตสำเร็จ!");
            Debug.Log("พยายามซื้อยูนิตด้วยทรัพยากรทีม: " + team);

        }
        else
        {
            Debug.Log("ไม้ไม่พอ!");
        }
    }

    // ฟังก์ชันสำหรับรับความเสียหาย
    public void TakeDamage(int damage)
    {
        health -= damage; // ลบพลังชีวิตของฐานจากความเสียหายที่ได้รับ
        if (health <= 0) // ถ้าพลังชีวิตของฐานเหลือ 0 หรือ น้อยกว่า
        {
            health = 0; // ตั้งค่า health เป็น 0
            Debug.Log("Base Destroyed!"); // แสดงข้อความว่า "ฐานถูกทำลาย"
        }

        UpdateHealthUI(); // อัปเดต UI เมื่อพลังชีวิตเปลี่ยนแปลง
    }

    // ฟังก์ชันอัปเดต UI
    private void UpdateHealthUI()
    {
        if (healthTextTMP != null)
        {
            healthTextTMP.text = "Base HP: " + health.ToString(); // แสดงค่า HP ที่ UI
        }
    }

    // ฟังก์ชันซ่อม
    public void Repair()
    {
        health = Mathf.Min(health + 20, 100);  // ซ่อมแซมให้ไม่เกิน 100
        Debug.Log(name + " กำลังซ่อมอาคาร, Health เหลือ: " + health);
    }
    public void SpawnUnit()
    {
        if (unitPrefab != null)
        {
            int maxAttempts = 10; // จำนวนครั้งสูงสุดในการหาตำแหน่งที่ว่าง
            for (int i = 0; i < maxAttempts; i++)
            {
                Vector2 spawnPosition = GetSpawnPosition();
                if (!IsPositionOccupied(spawnPosition))
                {
                    GameObject newUnit = Instantiate(unitPrefab, spawnPosition, Quaternion.identity);
                    newUnit.GetComponent<Unit>().SetTargetPosition(spawnPosition);
                    return; // ออกจากฟังก์ชันทันทีเมื่อเกิดสำเร็จ
                }
            }
            Debug.Log("ไม่สามารถหาตำแหน่งว่างได้ใน " + maxAttempts + " ครั้ง");
        }
    }


    // ฟังก์ชันสร้าง ConstructionWorker
    public void SpawnConstructionWorker()
    {
        if (constructionWorkerPrefab != null)
        {
            GameObject newWorker = Instantiate(constructionWorkerPrefab, spawnPoint.position, Quaternion.identity);
            newWorker.GetComponent<Unit>().SetTargetPosition(spawnPoint.position + new Vector3(-2, 0, 0));
        }
    }
    
    private Vector2 GetSpawnPosition()
    {
        Vector2 randomOffset = new Vector2(Random.Range(-2f, 2f), Random.Range(-2f, 2f));
        return (Vector2)spawnPoint.position + randomOffset;
    }
    
    private bool IsPositionOccupied(Vector2 position)
    {
        Collider2D collider = Physics2D.OverlapCircle(position, 0.5f);
    
        Debug.Log("Checking position: " + position + " | Collider found: " + (collider != null));

        return collider != null; // ถ้าพบคอลลิเดอร์ แสดงว่ามีสิ่งกีดขวาง
    }
    
    public void StartPlacingBuilding()
    {
        // ตรวจสอบว่ามีทรัพยากรพอหรือไม่สำหรับการวางอาคาร
        if (ResourceManager.instance.SpendResources(team, (int)unitCost)) // ตรวจสอบก่อนว่ามีทรัพยากรพอไหม
        {
            isPlacingBuilding = true;
            previewInstance = Instantiate(buildingPreview); // สร้างตัวอย่างของอาคาร
        }
        else
        {
            Debug.Log("เงินไม่พอสร้างอาคาร!");
        }
    }

    /*void PlaceBuilding(Vector2 position)
    {
        if (CanPlaceBuilding(position)) // ตรวจสอบว่าพื้นที่ที่เลือกสามารถวางอาคารได้หรือไม่
        {
            isPlacingBuilding = false;
            Destroy(previewInstance); // ลบตัวอย่างของอาคารหลังจากวาง

            // ตอนนี้ถึงเวลาหักเงิน
            if (ResourceManager.instance.SpendResources(team, (int)unitCost)) // หักเงินจริงๆ หลังจากวางอาคาร
            {
                GameObject newBuilding = Instantiate(buildingPrefab, position, Quaternion.identity); // สร้างอาคารจริง

                // หา ConstructionWorker ที่ใกล้ที่สุดเพื่อให้มันไปสร้างอาคาร
                ConstructionWorker worker = FindClosestWorker(position);
                if (worker != null)
                {
                    worker.SetTarget(position, newBuilding); // ให้ worker ไปทำงานที่ตำแหน่ง
                }
            }
            else
            {
                Debug.Log("ทรัพยากรไม่พอสำหรับการสร้างอาคาร");
            }
        }
        else
        {
            Debug.Log("พื้นที่นี้ไม่สามารถสร้างอาคารได้!");
        }
    }*/
    void PlaceBuilding(Vector2 position)
    {
        // ตรวจสอบก่อนว่าเราสามารถวางอาคารได้ในตำแหน่งนี้หรือไม่
        if (CanPlaceBuilding(position))
        {
            // วางอาคารในตำแหน่งที่ต้องการ
            Instantiate(buildingPrefab, position, Quaternion.identity);
            Debug.Log("วางอาคารสำเร็จ");
        }
        else
        {
            Debug.Log("ไม่สามารถวางอาคารที่ตำแหน่งนี้");
        }
    }

    
    // ฟังก์ชันยกเลิกการวางอาคาร
    private void CancelPlacingBuilding()
    {
        isPlacingBuilding = false; // ตั้งค่าให้ไม่อยู่ในโหมดการวางอาคาร
        if (previewInstance != null)
        {
            Destroy(previewInstance); // ลบตัวอย่างอาคารที่ยังไม่ได้วาง
        }
        Debug.Log("การสร้างอาคารถูกยกเลิก");
    }

    /*bool CanPlaceBuilding(Vector2 position)
    {
        Collider2D hit = Physics2D.OverlapCircle(position, 1f);
        return hit == null; // ถ้าไม่มีสิ่งกีดขวาง
    }*/
    bool CanPlaceBuilding(Vector2 position)
    {
        // กำหนดขนาดวงกลมที่ใช้ตรวจสอบ
        float radius = 1f; // เปลี่ยนขนาดให้เหมาะสมกับขนาดของอาคาร

        // ตรวจสอบว่าในตำแหน่งนั้นมีการชนกับ collider หรือไม่
        Collider2D hit = Physics2D.OverlapCircle(position, radius, LayerMask.GetMask("Ground"));

        // ถ้ามีการชนกับ collider ใน Layer "Ground"
        if (hit != null)
        {
            // ตรวจสอบว่า collider นี้มี Tag "Ground"
            if (hit.CompareTag("Ground"))
            {
                // ตรวจสอบว่า collider นี้เป็น trigger หรือไม่
                if (hit.isTrigger)
                {
                    // สามารถวางอาคารได้
                    Debug.Log("สามารถวางอาคารได้ที่ตำแหน่งนี้");
                    return true;
                }
                else
                {
                    // ถ้า collider ไม่ใช่ trigger
                    Debug.Log("พื้นไม่ใช่ Trigger");
                    return false;
                }
            }
            else
            {
                Debug.Log("พื้นที่ไม่ใช่ Tag 'Ground'");
                return false; // ถ้าไม่ใช่พื้นที่ที่สามารถวางได้
            }
        }
        else
        {
            // ไม่มี collider ที่ตรงกับตำแหน่งนี้
            Debug.Log("ไม่มีพื้นที่สามารถวางอาคารได้");
            return false;
        }
    }

    ConstructionWorker FindClosestWorker(Vector2 position)
    {
        ConstructionWorker[] workers = FindObjectsOfType<ConstructionWorker>();
        ConstructionWorker closest = null;
        float minDist = Mathf.Infinity;

        foreach (ConstructionWorker worker in workers)
        {
            float dist = Vector2.Distance(worker.transform.position, position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = worker;
            }
        }
        return closest;
    }
}
