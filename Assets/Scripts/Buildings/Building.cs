using UnityEngine;
using TMPro;
using Unity.Netcode;

public class Building : NetworkBehaviour
{
    public int team; // 0 = Team A, 1 = Team B
    public NetworkVariable<int> health = new NetworkVariable<int>(1000);
    public TextMeshProUGUI healthTextTMP;

    private void Start()
    {
        UpdateHealthUI();
    }

    public override void OnNetworkSpawn()
    {
        health.OnValueChanged += (oldVal, newVal) => UpdateHealthUI();
        UpdateHealthUI();
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

    [ServerRpc]
    public void RepairServerRpc()
    {
        health.Value = Mathf.Min(health.Value + 20, 1000);
        Debug.Log(name + " ซ่อมอาคาร, Health เหลือ: " + health.Value);
    }

    private void UpdateHealthUI()
    {
        if (healthTextTMP != null)
        {
            healthTextTMP.text = "Base HP: " + health.Value;
        }
    }
    
}
