using TMPro;
using UnityEngine;
using Unity.Netcode;

public class Unit : NetworkBehaviour
{
    public int team;
    [SerializeField] public float moveSpeed = 5f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    [SerializeField] public float fireRate = 1f;
    private float nextFireTime;

    private Vector2 targetPosition;
    private bool hasTarget = false;
    //[SerializeField] public float health = 100f; // ตัวแปร health สำหรับเก็บพลังชีวิตของยูนิต
    public NetworkVariable<float> health = new NetworkVariable<float>(100f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public TextMeshProUGUI healthTextTMP; // ถ้าใช้ TextMeshPro
    
    private Quaternion initialRotation; // เก็บค่าหมุนเริ่มต้นของยูนิต

    private void Start()
    {
        targetPosition = transform.position;
        initialRotation = transform.rotation;

        if (healthTextTMP == null)
        {
            healthTextTMP = GetComponentInChildren<TextMeshProUGUI>();
        }

        UpdateHealthUI(); 
    }

    private void Update()
    {
        if (!IsOwner) return;

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
            healthTextTMP.text = "HP: " + health.Value.ToString("0");
        }
    }

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
        bool targetFound = false;

        foreach (var enemy in enemies)
        {
            bool isEnemy = false;
            if (enemy.TryGetComponent<Unit>(out var enemyUnit) && enemyUnit.team != team) isEnemy = true;
            if (enemy.TryGetComponent<Base>(out var enemyBase) && enemyBase.team != team) isEnemy = true;
            if (enemy.TryGetComponent<Building>(out var enemyBuilding) && enemyBuilding.team != team) isEnemy = true;
            if (enemy.TryGetComponent<ConstructionWorker>(out var enemyWorker) && enemyWorker.team != team) isEnemy = true;

            if (isEnemy && Time.time >= nextFireTime)
            {
                RotateUnit(enemy.transform.position);
                FireServerRpc(enemy.transform.position);
                nextFireTime = Time.time + 1f / fireRate;
                targetFound = true;
                break;
            }
        }

        if (!targetFound && Time.time >= nextFireTime)
        {
            transform.rotation = initialRotation;
        }
    }


    private void RotateUnit(Vector2 target)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    [ServerRpc(RequireOwnership = false)]  // เพิ่ม RequireOwnership = false เพื่อให้ Client ที่ไม่เป็นเจ้าของสามารถยิงได้
    private void FireServerRpc(Vector2 target)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        Vector2 direction = (target - (Vector2)firePoint.position).normalized;

        bullet.GetComponent<Bullet>().Initialize(direction, (Team)team);

        if (rb != null)
        {
            rb.velocity = direction * 10f;
        }

        NetworkObject bulletNetObj = bullet.GetComponent<NetworkObject>();
        if (bulletNetObj != null)
        {
            bulletNetObj.Spawn(true); // Ensure the bullet is spawned across all clients
        }
    }


    public void SetTargetPosition(Vector2 newTarget)
    {
        targetPosition = newTarget;
        hasTarget = true;
    }

    public void TakeDamage(float damage)
    {
        if (IsServer) // ให้ลด HP บน Server เท่านั้น
        {
            health.Value -= damage;
            Debug.Log(name + " ได้รับความเสียหาย: " + damage + ", Health เหลือ: " + health.Value);
            if (health.Value <= 0)
            {
                Destroy(gameObject);
                Debug.Log(name + " ตาย!");
            }
        }
    }
    
    public override void OnNetworkSpawn()
    {
        health.OnValueChanged += (oldValue, newValue) =>
        {
            UpdateHealthUI();
        };

        UpdateHealthUI(); // ให้แน่ใจว่า UI ถูกเซ็ตตอน Spawn
    }


}
