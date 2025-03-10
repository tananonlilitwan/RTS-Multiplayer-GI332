/*
using System.Collections;
using TMPro;
using UnityEngine;

public class ConstructionWorker : Unit
{
    public int team;
    [SerializeField] public float moveSpeed = 5f;
    public GameObject buildingPrefab;
    public Transform buildPoint;
    public float buildSpeed = 1f;
    private float nextActionTime;

    public float chopTime = 1f; // เวลาที่ใช้ในการตัดแต่ละครั้ง
    public int woodGain = 50;
    public float relocateRadius = 5f;
    public LayerMask obstacleLayer;

    private Vector2 targetPosition;
    private bool hasTarget = false; 
    [SerializeField] public float health = 100f;
    public TextMeshProUGUI healthTextTMP;
    private ResourceManager resourceManager;
    private Tree targetTree;
    public int chopDamage = 10; // ความแรงในการตัดต้นไม้

    private void Start()
    {
        targetPosition = transform.position;
        UpdateHealthUI();
        resourceManager = FindObjectOfType<ResourceManager>();
    }

    private void Update()
    {
        if (hasTarget)
        {
            MoveUnit();  // เรียกใช้งานการเคลื่อนที่
        }
        if (IsPositionAvailable(targetPosition))
        {
            // เคลื่อนที่ไปยังตำแหน่งเป้าหมาย
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
        else
        {
            // ถ้าตำแหน่งมีอุปสรรค ลองหาตำแหน่งใหม่
            Vector2 newPosition = FindNewTargetPosition(targetPosition);
            SetTargetPosition(newPosition);
        }

        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            hasTarget = false;  // เมื่อถึงเป้าหมาย
        }
        
        HandleBuildingAndRepair();
    }

    private void MoveUnit()
    {
        Debug.Log("Target Position: " + targetPosition);  // เพิ่ม Debug เพื่อดูค่าของ targetPosition
        if (IsPositionAvailable(targetPosition))
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
        else
        {
            Debug.Log(name + " มียูนิตอื่นขวางทาง, ไม่สามารถไปยังตำแหน่งนั้นได้");
            // ลองหาเส้นทางใหม่หรือลองเคลื่อนไปในทิศทางที่ไม่มีอุปสรรค
        }
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            hasTarget = false;
            Debug.Log(name + " ถึงเป้าหมายแล้ว");
        }
    }

    private void UpdateHealthUI()
    {
        if (healthTextTMP != null)
        {
            healthTextTMP.text = "HP: " + health.ToString();
        }
    }

    private void HandleBuildingAndRepair()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, 3f);
        foreach (var target in targets)
        {
            Building building = target.GetComponent<Building>();
            Base baseUnit = target.GetComponent<Base>();
            Tree tree = target.GetComponent<Tree>();

            if (tree != null && targetTree == null) // ถ้ายังไม่มีเป้าหมาย ให้เลือกต้นไม้ที่เจอก่อน
            {
                targetTree = tree;
                StartCoroutine(ChopTree());
            }
            else if ((building != null && building.team == team) || (baseUnit != null && baseUnit.team == team))
            {
                if (Time.time >= nextActionTime)
                {
                    RepairTarget(target);
                    nextActionTime = Time.time + 1f / buildSpeed;
                }
            }
        }
    }

    // ฟังก์ชันตัดต้นไม้
    private IEnumerator ChopTree()
    {
        while (targetTree != null)
        {
            targetTree.Chop(chopDamage);
            yield return new WaitForSeconds(chopTime); // รอเวลาตัด
            if (targetTree.health <= 0) // ถ้าต้นไม้ตาย
            {
                Vector2 newPosition = FindNewTreePosition(targetTree.transform.position);
                targetTree.transform.position = newPosition; // ย้ายต้นไม้
                targetTree.ResetHealth(); // รีเซ็ต HP
                targetTree = null; // ล้างค่าเป้าหมาย
            }
        }
    }

    // ตรวจจับต้นไม้รอบๆ (ใช้ 2D)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Tree") && targetTree == null)
        {
            targetTree = other.GetComponent<Tree>();
            StartCoroutine(ChopTree()); // เริ่มตัดต้นไม้
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Tree") && other.GetComponent<Tree>() == targetTree)
        {
            targetTree = null;
        }
    }

    private Vector2 FindNewTreePosition(Vector2 oldPosition)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * relocateRadius;
            Vector2 newPosition = oldPosition + randomOffset;
            if (!Physics2D.OverlapCircle(newPosition, 1f, obstacleLayer))
            {
                return newPosition;
            }
        }
        return oldPosition; // ถ้าหาที่ใหม่ไม่ได้
    }

    private void RepairTarget(Collider2D target)
    {
        Building building = target.GetComponent<Building>();
        Base baseUnit = target.GetComponent<Base>();
        if (building != null)
        {
            building.Repair();
        }
        else if (baseUnit != null)
        {
            baseUnit.Repair();
        }
    }

    public void SetTargetPosition(Vector2 newTarget)
    {
        if (IsPositionAvailable(newTarget)) // ตรวจสอบว่าตำแหน่งไม่มีอุปสรรค
        {
            targetPosition = newTarget;
            hasTarget = true;
        }
        else
        {
            Debug.Log("ตำแหน่งเป้าหมายมีอุปสรรค, กำลังหาตำแหน่งใหม่");
            // ถ้าตำแหน่งไม่ว่างให้หาตำแหน่งใหม่
            Vector2 newPosition = FindNewTargetPosition(newTarget);
           // SetTargetPosition(newPosition);
            if (newPosition != newTarget)
            {
                SetTargetPosition(newPosition);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        UpdateHealthUI();
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
    
    /*public void ResetTreeHealth()
    {
        if (targetTree != null)
        {
            targetTree.ResetHealth();
        }
    }#1#
    

    private bool IsPositionAvailable(Vector2 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.5f);
        foreach (var collider in colliders)
        {
            Unit unit = collider.GetComponent<Unit>();
            if (unit != null && unit != this)
            {
                Debug.Log("ตำแหน่งมีอุปสรรค: " + unit.name);  // ตรวจสอบว่ามีอุปสรรคหรือไม่
                return false;
            }
            
            Base baseUnit = collider.GetComponent<Base>();
            if (baseUnit != null && baseUnit != this)
            {
                return false;
            }

            Building buildingUnit = collider.GetComponent<Building>();
            if (buildingUnit != null && buildingUnit != this)
            {
                return false;
            }
            
            ConstructionWorker ConstructionWorkerUnit = collider.GetComponent<ConstructionWorker>();
            if (ConstructionWorkerUnit != null && ConstructionWorkerUnit != this)
            {
                return false;
            }
        }
        return true; // ถ้าตำแหน่งว่าง
    }


    private Vector2 FindNewTargetPosition(Vector2 currentPosition)
    {
        // พยายามหาตำแหน่งใหม่ที่ไม่ขวางทาง
        for (int i = 0; i < 10; i++) // พยายามหาตำแหน่งใหม่ 10 ครั้ง
        {
            Vector2 randomOffset = Random.insideUnitCircle * 2f; // หาตำแหน่งใหม่ที่อยู่ใกล้ๆ
            Vector2 newPosition = currentPosition + randomOffset;

            if (IsPositionAvailable(newPosition)) // ตรวจสอบว่าไม่มีอุปสรรค
            {
                return newPosition;
            }
        }

        // ถ้าไม่สามารถหาตำแหน่งใหม่ได้ ก็จะกลับไปที่ตำแหน่งเดิม
        return currentPosition;
    }
}
*/


using System.Collections;
using TMPro;
using UnityEngine;

public class ConstructionWorker : MonoBehaviour
{
    public int team;
    [SerializeField] public float moveSpeed = 5f;
    public GameObject buildingPrefab;
    public Transform buildPoint;
    public float buildSpeed = 1f;
    private float nextActionTime;

    public float chopTime = 1f; // เวลาที่ใช้ในการตัดแต่ละครั้ง
    public int woodGain = 50;
    public float relocateRadius = 5f;
    public LayerMask obstacleLayer;

    private Vector2 targetPosition;
    private bool hasTarget = false;
    [SerializeField] public float health = 100f;
    public TextMeshProUGUI healthTextTMP;
    private ResourceManager resourceManager;
    private Tree targetTree;
    public int chopDamage = 10; // ความแรงในการตัดต้นไม้

    private void Start()
    {
        targetPosition = transform.position;
        UpdateHealthUI();
        resourceManager = FindObjectOfType<ResourceManager>();
    }

    private void Update()
    {
        if (hasTarget)
        {
            MoveUnit();
        }
        HandleBuildingAndRepair();
    }

    private void MoveUnit()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            hasTarget = false;
        }
    }

    private void UpdateHealthUI()
    {
        if (healthTextTMP != null)
        {
            healthTextTMP.text = "HP: " + health.ToString();
        }
    }

    private void HandleBuildingAndRepair()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, 3f);
        foreach (var target in targets)
        {
            Building building = target.GetComponent<Building>();
            Base baseUnit = target.GetComponent<Base>();
            Tree tree = target.GetComponent<Tree>();

            if (tree != null && targetTree == null) // ถ้ายังไม่มีเป้าหมาย ให้เลือกต้นไม้ที่เจอก่อน
            {
                targetTree = tree;
                StartCoroutine(ChopTree());
            }
            else if ((building != null && building.team == team) || (baseUnit != null && baseUnit.team == team))
            {
                if (Time.time >= nextActionTime)
                {
                    RepairTarget(target);
                    nextActionTime = Time.time + 1f / buildSpeed;
                }
            }
        }
    }

    // ฟังก์ชันตัดต้นไม้
    /*private IEnumerator ChopTree()
    {
        while (targetTree != null)
        {
            targetTree.Chop(chopDamage);
            yield return new WaitForSeconds(chopTime); // รอเวลาตัด
            if (targetTree.health <= 0) // ถ้าต้นไม้ตาย
            {
                Vector2 newPosition = FindNewTreePosition(targetTree.transform.position);
                targetTree.transform.position = newPosition; // ย้ายต้นไม้
                targetTree.ResetHealth(); // รีเซ็ต HP
                targetTree = null; // ล้างค่าเป้าหมาย
            }
        }
    }*/
    private IEnumerator ChopTree()
    {
        while (targetTree != null)
        {
            targetTree.Chop(chopDamage, team); // ส่งค่า team ไปด้วย
            yield return new WaitForSeconds(chopTime);
            if (targetTree.health <= 0)
            {
                Vector2 newPosition = FindNewTreePosition(targetTree.transform.position);
                targetTree.transform.position = newPosition;
                targetTree.ResetHealth();
                targetTree = null;
            }
        }
    }


    // ตรวจจับต้นไม้รอบๆ (ใช้ 2D)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Tree") && targetTree == null)
        {
            targetTree = other.GetComponent<Tree>();
            StartCoroutine(ChopTree()); // เริ่มตัดต้นไม้
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Tree") && other.GetComponent<Tree>() == targetTree)
        {
            targetTree = null;
        }
    }

    private Vector2 FindNewTreePosition(Vector2 oldPosition)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * relocateRadius;
            Vector2 newPosition = oldPosition + randomOffset;
            if (!Physics2D.OverlapCircle(newPosition, 1f, obstacleLayer))
            {
                return newPosition;
            }
        }
        return oldPosition; // ถ้าหาที่ใหม่ไม่ได้
    }

    private void RepairTarget(Collider2D target)
    {
        Building building = target.GetComponent<Building>();
        Base baseUnit = target.GetComponent<Base>();
        if (building != null)
        {
            building.Repair();
        }
        else if (baseUnit != null)
        {
            baseUnit.Repair();
        }
    }

    public void SetTargetPosition(Vector2 newTarget)
    {
        targetPosition = newTarget;
        hasTarget = true;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        UpdateHealthUI();
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
    
    public void ResetTreeHealth()
    {
        if (targetTree != null)
        {
            targetTree.ResetHealth();
        }
    }



}