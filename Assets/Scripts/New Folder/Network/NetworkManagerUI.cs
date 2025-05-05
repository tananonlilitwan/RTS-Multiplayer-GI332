// NetworkManagerUI.cs
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkManagerUI : MonoBehaviour
{
    public Button hostButton;
    public Button clientButton;
    public GameObject teamSelectPanel;
    public Button teamAButton;
    public Button teamBButton;

    public static int selectedTeam = 0; // 0 = A, 1 = B

    private void Awake()
    {
        hostButton.onClick.AddListener(() => NetworkManager.Singleton.StartHost());
        clientButton.onClick.AddListener(() => NetworkManager.Singleton.StartClient());

        teamAButton.onClick.AddListener(() =>
        {
            selectedTeam = 0;
            teamSelectPanel.SetActive(false);
        });

        teamBButton.onClick.AddListener(() =>
        {
            selectedTeam = 1;
            teamSelectPanel.SetActive(false);
        });
    }
}