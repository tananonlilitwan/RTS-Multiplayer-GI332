using TMPro;
using UnityEngine;
using Unity.Netcode;

public class Tree : NetworkBehaviour
{
    public float health = 100f;
    private float maxHealth;

    public float minX = -10f, maxX = 10f, minY = -10f, maxY = 10f;
    public LayerMask obstacleLayer;
    public int woodReward = 50;

    public TextMeshProUGUI healthText;

    private NetworkVariable<float> syncedHealth = new NetworkVariable<float>(100f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Start()
    {
        maxHealth = health;
        
        syncedHealth.Value = health;

        if (healthText == null)
            healthText = GetComponentInChildren<TextMeshProUGUI>();

        UpdateHealthUI();

        // Subscribe เมื่อตัวเลข HP เปลี่ยน
        syncedHealth.OnValueChanged += (oldVal, newVal) =>
        {
            UpdateHealthUI();
        };
    }

    public void Chop(int damage, int team)
    {
        if (!IsServer) return;

        health -= damage;
        syncedHealth.Value = health;

        if (health <= 0)
        {
            ResourceManager.instance.AddResources(woodReward);
            RelocateTree();
        }
    }

    public void ResetHealth()
    {
        health = maxHealth;
        syncedHealth.Value = maxHealth;
    }

    private void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = "HP: " + Mathf.Max(0, syncedHealth.Value).ToString("0");
        }
    }

    private void RelocateTree()
    {
        Vector2 newPosition = GetSafePosition();
        transform.position = newPosition;
        ResetHealth();
    }

    private Vector2 GetSafePosition()
    {
        int maxAttempts = 100;
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY),
                0f
            );

            Collider2D[] colliders = Physics2D.OverlapCircleAll(randomPosition, 1f, obstacleLayer);
            if (colliders.Length == 0)
            {
                return randomPosition;
            }
        }

        Debug.LogWarning("ไม่สามารถหาตำแหน่งใหม่ได้ ใช้ตำแหน่งเดิม");
        return transform.position;
    }
}
