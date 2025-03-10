using TMPro; // ถ้าใช้ TextMeshPro
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int team;
    [SerializeField] public float moveSpeed = 5f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    [SerializeField] public float fireRate = 1f;
    private float nextFireTime;

    private Vector2 targetPosition;
    private bool hasTarget = false;
    [SerializeField] public float health = 100f; // ตัวแปร health สำหรับเก็บพลังชีวิตของยูนิต
    public TextMeshProUGUI healthTextTMP; // ถ้าใช้ TextMeshPro
    
    private Quaternion initialRotation; // เก็บค่าหมุนเริ่มต้นของยูนิต

    
    
    private void Start()
    {
        targetPosition = transform.position;
        initialRotation = transform.rotation; // บันทึกค่าหมุนเริ่มต้น
        UpdateHealthUI(); // เรียกฟังก์ชันเพื่ออัปเดต UI ตอนเริ่มเกม
    }

    private void Update()
    {
        if (hasTarget)
        {
            MoveUnit();
        }
        DetectAndFire();
    }

    private void MoveUnit()
    {
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
            healthTextTMP.text = "HP: " + health.ToString(); // ถ้าใช้ TextMeshPro
        }
    }

    // ฟังก์ชันที่ตรวจสอบว่าตำแหน่งนั้นมียูนิตอื่นๆ ขวางอยู่หรือไม่
    private bool IsPositionAvailable(Vector2 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.5f);
        foreach (var collider in colliders)
        {
            Unit unit = collider.GetComponent<Unit>();
            if (unit != null && unit != this)
            {
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
        return true;
    }


    private void DetectAndFire()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 5f);
        bool targetFound = false; // ตัวแปรที่ใช้ตรวจสอบว่าพบเป้าหมายหรือไม่

        foreach (var enemy in enemies)
        {
            Unit enemyUnit = enemy.GetComponent<Unit>();
            Base enemyBase = enemy.GetComponent<Base>();
            Building enemyBuilding = enemy.GetComponent<Building>();
            ConstructionWorker enemyConstructionWorker = enemy.GetComponent<ConstructionWorker>();

            // ตรวจสอบว่าเป็นยูนิตที่เป็นศัตรู
            if (enemyUnit != null && enemyUnit.team != team)
            {
                if (Time.time >= nextFireTime)
                {
                    RotateUnit(enemy.transform.position);  // หมุนไปที่ทิศทางของศัตรู
                    Fire(enemy.transform.position);
                    nextFireTime = Time.time + 1f / fireRate;
                    targetFound = true; // พบเป้าหมาย
                }
            }
            // ตรวจสอบว่าเป็นฐานของศัตรู
            else if (enemyBase != null && enemyBase.team != team)
            {
                if (Time.time >= nextFireTime)
                {
                    RotateUnit(enemy.transform.position);  // หมุนไปที่ทิศทางของศัตรู
                    Fire(enemy.transform.position);
                    nextFireTime = Time.time + 1f / fireRate;
                    targetFound = true; // พบเป้าหมาย
                }
            }
            // ตรวจสอบว่าเป็นอาคารของศัตรู
            else if (enemyBuilding != null && enemyBuilding.team != team)
            {
                if (Time.time >= nextFireTime)
                {
                    RotateUnit(enemy.transform.position);  // หมุนไปที่ทิศทางของศัตรู
                    Fire(enemy.transform.position);
                    nextFireTime = Time.time + 1f / fireRate;
                    targetFound = true; // พบเป้าหมาย
                }
            }
            // ตรวจสอบว่าเป็นคนงานของศัตรู
            if (enemyConstructionWorker != null && enemyConstructionWorker.team != team)
            {
                if (Time.time >= nextFireTime)
                {
                    RotateUnit(enemy.transform.position);  // หมุนไปที่ทิศทางของศัตรู
                    Fire(enemy.transform.position);
                    nextFireTime = Time.time + 1f / fireRate;
                    targetFound = true; // พบเป้าหมาย
                }
            }
        }

        // หากไม่พบเป้าหมายและไม่มีการยิง, ให้หมุนกลับไปที่ค่าตั้งต้น
        if (!targetFound && Time.time >= nextFireTime)
        {
            transform.rotation = initialRotation; // รีเซ็ตการหมุนกลับค่าตั้งต้น
        }
    }



    private void RotateUnit(Vector2 target)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void Fire(Vector2 target)
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("Bullet หรือ FirePoint ไม่ได้ตั้งค่า!");
            return;
        }

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        Vector2 direction = (target - (Vector2)firePoint.position).normalized;

        if (rb != null)
        {
            rb.velocity = direction * 10f;
        }
        else
        {
            bullet.transform.position += (Vector3)(direction * 10f * Time.deltaTime);
        }

        Debug.Log(name + " ยิงไปที่ " + target);
    }

    public void SetTargetPosition(Vector2 newTarget)
    {
        targetPosition = newTarget;
        hasTarget = true;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log(name + " ได้รับความเสียหาย: " + damage + ", Health เหลือ: " + health);
        UpdateHealthUI(); // อัปเดต UI เมื่อได้รับความเสียหาย
        if (health <= 0)
        {
            Destroy(gameObject);
            Debug.Log(name + " ตาย!");
        }
    }
}