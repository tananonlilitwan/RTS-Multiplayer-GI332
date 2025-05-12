using TMPro;
using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class ConstructionWorker : NetworkBehaviour
{
    public int team;
    [SerializeField] public float moveSpeed = 5f;
    public GameObject buildingPrefab;
    public Transform buildPoint;
    public float buildSpeed = 1f;
    private float nextActionTime;

    public float chopTime = 1f;
    public int woodGain = 50;
    public float relocateRadius = 5f;
    public LayerMask obstacleLayer;

    private Vector2 targetPosition;
    private bool hasTarget = false;

    public NetworkVariable<float> health = new NetworkVariable<float>(100f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public TextMeshProUGUI healthTextTMP;
    private ResourceManager resourceManager;
    private Tree targetTree;
    public int chopDamage = 10;

    private void Start()
    {
        targetPosition = transform.position;
        resourceManager = FindObjectOfType<ResourceManager>();

        if (healthTextTMP == null)
            healthTextTMP = GetComponentInChildren<TextMeshProUGUI>();

        UpdateHealthUI();
    }

    private void Update()
    {
        if (!IsOwner) return;

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
            healthTextTMP.text = "HP: " + health.Value.ToString("0");
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

            if (tree != null && targetTree == null)
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

    private IEnumerator ChopTree()
    {
        while (targetTree != null)
        {
            if (IsServer)
            {
                ChopTreeServerRpc(targetTree.NetworkObjectId, chopDamage, team);
                targetTree.Chop(chopDamage, team);
            }

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

    [ServerRpc]
    private void ChopTreeServerRpc(ulong treeId, int damage, int team)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(treeId, out var netObj))
        {
            Tree tree = netObj.GetComponent<Tree>();
            if (tree != null)
            {
                tree.Chop(damage, team);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Tree") && targetTree == null)
        {
            targetTree = other.GetComponent<Tree>();
            StartCoroutine(ChopTree());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Tree") && other.GetComponent<Tree>() == targetTree)
        {
            targetTree = null;
        }
    }

    private void RepairTarget(Collider2D target)
    {
        if (!IsServer) return;

        Building building = target.GetComponent<Building>();
        Base baseUnit = target.GetComponent<Base>();

        if (building != null)
        {
            //building.Repair();
            building.RepairServerRpc();
        }
        else if (baseUnit != null)
        {
            baseUnit.Repair();
        }
    }

    public void SetTargetPosition(Vector2 newTarget)
    {
        if (!IsOwner) return;

        targetPosition = newTarget;
        hasTarget = true;
    }

    public void TakeDamage(float damage)
    {
        if (IsServer)
        {
            health.Value -= damage;
            Debug.Log(name + " ถูกโจมตี: -" + damage + " HP เหลือ: " + health.Value);

            if (health.Value <= 0)
            {
                NetworkObject.Despawn();
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        health.OnValueChanged += (oldVal, newVal) =>
        {
            UpdateHealthUI();
        };

        UpdateHealthUI();
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
}
