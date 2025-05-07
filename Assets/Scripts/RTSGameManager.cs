/*
using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;


public class RTSGameManager : MonoBehaviour
{
    public static RTSGameManager Instance; // ตัวแปรสำหรับเก็บอินสแตนซ์ของ GameManager

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
        if (Instance == null) // ถ้ายังไม่มีอินสแตนซ์ของ GameManager
            Instance = this; // กำหนดให้ตัวเองเป็นอินสแตนซ์เดียว
    }

    private void Update()
    {
        //CheckWinCondition(); // ตรวจสอบเงื่อนไขการชนะในทุกๆ เฟรม
        
        if (!NetworkManager.Singleton.IsServer) return; // ตรวจเฉพาะ Host
        CheckWinCondition();
    }

    private void CheckWinCondition()
    {
        if (gameEnded) return;
        
        int playerACaptured = 0; // จำนวนพื้นที่ที่ผู้เล่น A ยึดได้
        int playerBCaptured = 0; // จำนวนพื้นที่ที่ผู้เล่น B ยึดได้
        
        foreach (var point in territoryPoints) // ลูปผ่านทุกพื้นที่
        {
            if (point.currentOwner == 0) playerACaptured++; // ถ้าผู้เล่น A เป็นเจ้าของพื้นที่ เพิ่มจำนวนพื้นที่ที่ผู้เล่น A ยึด
            if (point.currentOwner == 1) playerBCaptured++; // ถ้าผู้เล่น B เป็นเจ้าของพื้นที่ เพิ่มจำนวนพื้นที่ที่ผู้เล่น B ยึด
            

        }

        /*if (playerACaptured >= 2) // ถ้าผู้เล่น A ยึดพื้นที่ได้มากกว่าหรือเท่ากับ 2 แห่ง
        {
            Debug.Log("Player A Wins!"); // ผู้เล่น A ชนะ
        }
        else if (playerBCaptured >= 2) // ถ้าผู้เล่น B ยึดพื้นที่ได้มากกว่าหรือเท่ากับ 2 แห่ง
        {
            Debug.Log("Player B Wins!"); // ผู้เล่น B ชนะ
        }
        else if (headquartersB.health.Value <= 0)//else if (headquartersA.health <= 0) // ถ้าฐานของผู้เล่น A ถูกทำลาย
        {
            Debug.Log("Player B Wins! (HQ Destroyed)"); // ผู้เล่น B ชนะเพราะทำลายฐานของผู้เล่น A
        }
        else if (headquartersA.health.Value <= 0)//else if (headquartersB.health <= 0) // ถ้าฐานของผู้เล่น B ถูกทำลาย
        {
            Debug.Log("Player A Wins! (HQ Destroyed)"); // ผู้เล่น A ชนะเพราะทำลายฐานของผู้เล่น B
        }#1#
        
        if (playerACaptured >= 2)
        {
            Debug.Log("Player A Wins!");
            gameEnded = true;
            AnnounceWinClientRpc(0); // 0 = Team A
        }
        else if (playerBCaptured >= 2)
        {
            Debug.Log("Player B Wins!");
            gameEnded = true;
            AnnounceWinClientRpc(1); // 1 = Team B
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
*/



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

