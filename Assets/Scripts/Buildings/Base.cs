using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;

public class Base : NetworkBehaviour
{
    public int team; // 0 สำหรับผู้เล่น A, 1 สำหรับผู้เล่น B
    public NetworkVariable<int> health = new NetworkVariable<int>(500, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public TextMeshProUGUI healthTextTMP;

    public GameObject unitPrefab;
    public GameObject constructionWorkerPrefab;
    public Transform spawnPoint;

    public float unitCost = 50f;

    public Button buyUnitButton;
    public Button buyConstructionWorkerButton;

    public GameObject buildingPrefab;
    public GameObject buildingPreview;
    private GameObject previewInstance;
    private bool isPlacingBuilding = false;

    private void Start()
    {
        UpdateHealthUI();

        buyUnitButton.onClick.AddListener(OnBuyUnitButtonClicked);
        buyConstructionWorkerButton.onClick.AddListener(OnBuyConstructionWorkerButtonClicked);
    }

    private void OnEnable()
    {
        health.OnValueChanged += OnHealthChanged;
    }

    private void OnDisable()
    {
        health.OnValueChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(int oldValue, int newValue)
    {
        UpdateHealthUI();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (isPlacingBuilding)
                CancelPlacingBuilding();
            else
                TryRightClickToBuyUnit();
        }

        if (isPlacingBuilding && previewInstance != null)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            previewInstance.transform.position = mousePos;

            if (Input.GetMouseButtonDown(0))
            {
                PlaceBuilding(mousePos);
            }
        }
    }

    private void TryRightClickToBuyUnit()
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(buyUnitButton.GetComponent<RectTransform>(), Input.mousePosition))
        {
            OnBuyUnitButtonClicked();
        }
        else if (RectTransformUtility.RectangleContainsScreenPoint(buyConstructionWorkerButton.GetComponent<RectTransform>(), Input.mousePosition))
        {
            OnBuyConstructionWorkerButtonClicked();
        }
    }

    private void OnBuyUnitButtonClicked()
    {
        BuyUnit(spawnPoint.position);
    }

    private void OnBuyConstructionWorkerButtonClicked()
    {
        SpawnConstructionWorker();
    }

    public void BuyUnit(Vector3 spawnPosition)
    {
        if (ResourceManager.instance.SpendResources(team, (int)unitCost))
        {
            GameObject newUnit = Instantiate(unitPrefab, spawnPosition, Quaternion.identity);
            newUnit.GetComponent<NetworkObject>().Spawn(); // network spawn
            newUnit.GetComponent<Unit>().SetTargetPosition(spawnPosition);
        }
        else
        {
            Debug.Log("ไม้ไม่พอ!");
        }
    }

    [ServerRpc]
    public void TakeDamageServerRpc(int damage)
    {
        health.Value -= damage;

        if (health.Value <= 0)
        {
            health.Value = 0;
            Debug.Log("Base Destroyed!");
        }
    }

    private void UpdateHealthUI()
    {
        if (healthTextTMP != null)
        {
            healthTextTMP.text = "Base HP: " + health.Value.ToString();
        }
    }

    public void Repair()
    {
        health.Value = Mathf.Min(health.Value + 20, 1000);
        Debug.Log(name + " กำลังซ่อมอาคาร, Health เหลือ: " + health.Value);
    }

    public void SpawnUnit()
    {
        if (unitPrefab != null)
        {
            int maxAttempts = 10;
            for (int i = 0; i < maxAttempts; i++)
            {
                Vector2 spawnPosition = GetSpawnPosition();
                if (!IsPositionOccupied(spawnPosition))
                {
                    GameObject newUnit = Instantiate(unitPrefab, spawnPosition, Quaternion.identity);
                    newUnit.GetComponent<NetworkObject>().Spawn(); // network spawn
                    newUnit.GetComponent<Unit>().SetTargetPosition(spawnPosition);
                    return;
                }
            }
            Debug.Log("ไม่สามารถหาตำแหน่งว่างได้ใน " + maxAttempts + " ครั้ง");
        }
    }

    public void SpawnConstructionWorker()
    {
        if (constructionWorkerPrefab != null)
        {
            GameObject newWorker = Instantiate(constructionWorkerPrefab, spawnPoint.position, Quaternion.identity);
            newWorker.GetComponent<NetworkObject>().Spawn(); // network spawn
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
        return collider != null;
    }

    public void StartPlacingBuilding()
    {
        if (ResourceManager.instance.SpendResources(team, (int)unitCost))
        {
            isPlacingBuilding = true;
            previewInstance = Instantiate(buildingPreview);
        }
        else
        {
            Debug.Log("เงินไม่พอสร้างอาคาร!");
        }
    }

    void PlaceBuilding(Vector2 position)
    {
        if (CanPlaceBuilding(position))
        {
            Instantiate(buildingPrefab, position, Quaternion.identity).GetComponent<NetworkObject>()?.Spawn();
            Debug.Log("วางอาคารสำเร็จ");
        }
        else
        {
            Debug.Log("ไม่สามารถวางอาคารที่ตำแหน่งนี้");
        }
    }

    private void CancelPlacingBuilding()
    {
        isPlacingBuilding = false;
        if (previewInstance != null)
        {
            Destroy(previewInstance);
        }
        Debug.Log("การสร้างอาคารถูกยกเลิก");
    }

    bool CanPlaceBuilding(Vector2 position)
    {
        float radius = 1f;
        Collider2D hit = Physics2D.OverlapCircle(position, radius, LayerMask.GetMask("Ground"));
        if (hit != null)
        {
            if (hit.CompareTag("Ground") && hit.isTrigger)
            {
                return true;
            }
        }
        return false;
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
    
    public void TakeDamage(int damage)
    {
        // ป้องกันไม่ให้ client เรียกเอง
        if (!IsServer) return;

        TakeDamageServerRpc(damage);
    }

}
