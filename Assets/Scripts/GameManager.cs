using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    public GameObject winPanel;
    public GameObject losePanel;
    
    

    private void Awake()
    {
        Instance = this;
        
        GameObject resetObj = GameObject.FindWithTag("ResetButton");
        if (resetObj != null)
        {
            var btn = resetObj.GetComponent<UnityEngine.UI.Button>();
            if (btn != null)
                btn.onClick.AddListener(OnResetButtonClicked);
        }
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
    
    public void OnResetButtonClicked()
    {
        if (IsServer)
        {
            ReloadSceneClientRpc();
        }
        else
        {
            RequestResetServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestResetServerRpc()
    {
        ReloadSceneClientRpc();
    }

    [ClientRpc]
    public void ReloadSceneClientRpc()
    {
        // เปลี่ยนเป็นชื่อ Scene ของคุณ เช่น "MainScene"
        SceneManager.LoadScene("RTS Multiplayer");
    }
}