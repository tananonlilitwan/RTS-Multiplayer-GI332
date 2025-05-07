using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    public GameObject winPanel;
    public GameObject losePanel;

    private void Awake()
    {
        Instance = this;
    }

    [ClientRpc]
    public void ShowWinPanelClientRpc(ClientRpcParams clientRpcParams = default)
    {
        if (!IsOwner) return;
        winPanel.SetActive(true);
    }

    [ClientRpc]
    public void ShowLosePanelClientRpc(ClientRpcParams clientRpcParams = default)
    {
        if (!IsOwner) return;
        losePanel.SetActive(true);
    }

    // วิธีเรียกให้เฉพาะ client ที่แพ้
    public void TriggerLosePanelForClient(ulong clientId)
    {
        var targetClient = new ClientRpcParams {
            Send = new ClientRpcSendParams {
                TargetClientIds = new[] { clientId }
            }
        };

        ShowLosePanelClientRpc(targetClient);
    }

    public void TriggerWinPanelForClient(ulong clientId)
    {
        var targetClient = new ClientRpcParams {
            Send = new ClientRpcSendParams {
                TargetClientIds = new[] { clientId }
            }
        };

        ShowWinPanelClientRpc(targetClient);
    }
}