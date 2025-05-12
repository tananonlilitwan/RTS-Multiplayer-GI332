using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;

public class RTSGameManager : MonoBehaviour
{
    public static RTSGameManager Instance;

    public List<TerritoryPoint> territoryPoints; // รายการของพื้นที่ที่สามารถถูกยึด
    public Base headquartersA; // ฐานของผู้เล่น A
    public Base headquartersB; // ฐานของผู้เล่น B
    public ResourceManager resourceManager; // ตัวจัดการทรัพยากร
    public UnitManager unitManager; // ตัวจัดการหน่วยทหาร
    public BuildingManager buildingManager; // ตัวจัดการอาคาร
    
    private bool gameEnded = false;

    [ClientRpc]
    void AnnounceWinClientRpc(int winningTeam)
    {
        int localTeam = NetworkManagerUI.selectedTeam;

        UIController ui = FindObjectOfType<UIController>();
        if (ui == null) return;

        if (localTeam == winningTeam)
        {
            ui.ShowWin(localTeam);
        }
        else
        {
            ui.ShowLose(localTeam);
        }
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Update()
    {
        if (!NetworkManager.Singleton.IsServer) return;
        CheckWinCondition();
    }
    
    public void MoveCameraToSpawn(int team)
    {
        Vector3 spawnPos = TeamSpawnPoint.GetSpawnPositionForTeam(team);
        spawnPos.z = Camera.main.transform.position.z;
        Camera.main.transform.position = spawnPos;
    }

    private void CheckWinCondition()
    {
        if (gameEnded) return;
        
        int playerACaptured = 0;
        int playerBCaptured = 0;
        
        foreach (var point in territoryPoints)
        {
            if (point.currentOwner.Value == 0) playerACaptured++;
            if (point.currentOwner.Value == 1) playerBCaptured++;
        }

        if (playerACaptured >= 2)
        {
            Debug.Log("Player A Wins!");
            gameEnded = true;
            AnnounceWinClientRpc(0); // Team A wins
        }
        else if (playerBCaptured >= 2)
        {
            Debug.Log("Player B Wins!");
            gameEnded = true;
            AnnounceWinClientRpc(1); // Team B wins
        }
        else if (headquartersB.health.Value <= 0)
        {
            Debug.Log("Player A Wins! (HQ Destroyed)");
            gameEnded = true;
            AnnounceWinClientRpc(0);
        }
        else if (headquartersA.health.Value <= 0)
        {
            Debug.Log("Player B Wins! (HQ Destroyed)");
            gameEnded = true;
            AnnounceWinClientRpc(1);
        }
    }
}

