using UnityEngine;
using UnityEngine.UI;

public class UnitSpawnerUI : MonoBehaviour
{
    public Base playerBase; // ฐานของผู้เล่น
    public Button spawnUnitButton;
    public Button spawnWorkerButton;

    private void Start()
    {
        spawnUnitButton.onClick.AddListener(playerBase.SpawnUnit);
        spawnWorkerButton.onClick.AddListener(playerBase.SpawnConstructionWorker);
    }
}