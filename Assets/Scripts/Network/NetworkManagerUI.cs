using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using TMPro;

public class NetworkManagerUI : MonoBehaviour
{
    public Button hostButton;
    public Button clientButton;
    public GameObject teamSelectPanel;
    public Button teamAButton;
    public Button teamBButton;
    
    public GameObject teamAPanel;
    public GameObject teamBPanel;
    
    public TMP_InputField ipInputField;
    
    public static int selectedTeam = 0; // 0 = A, 1 = B

    private void Awake()
    {
        hostButton.onClick.AddListener(() => NetworkManager.Singleton.StartHost());
        clientButton.onClick.AddListener(() => NetworkManager.Singleton.StartClient());
        
        hostButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
        });

        clientButton.onClick.AddListener(() =>
        {
            string ip = ipInputField.text.Trim();
            if (!string.IsNullOrEmpty(ip))
            {
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ip, 7777);
                NetworkManager.Singleton.StartClient();
            }
            else
            {
                Debug.LogWarning("IP address is empty!");
            }
        });
        

        teamAButton.onClick.AddListener(() =>
        {
            selectedTeam = 0;
            teamSelectPanel.SetActive(false);
            teamAPanel.SetActive(true);
            teamBPanel.SetActive(false);
            
        });

        teamBButton.onClick.AddListener(() =>
        {
            selectedTeam = 1;
            teamSelectPanel.SetActive(false);
            teamBPanel.SetActive(true);
            teamAPanel.SetActive(false);
        });
    }
    
    public void OnExitButtonPressed()
    { 
        Application.Quit();
    }
}