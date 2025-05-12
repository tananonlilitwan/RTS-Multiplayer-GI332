using UnityEngine;
using System.Collections.Generic;

public class TeamSpawnPoint : MonoBehaviour
{
    public int team; // 0 = Team A, 1 = Team B

    private static Dictionary<int, Vector3> spawnPoints = new Dictionary<int, Vector3>();

    private void Awake()
    {
        if (!spawnPoints.ContainsKey(team))
        {
            spawnPoints[team] = transform.position;
        }
    }

    public static Vector3 GetSpawnPositionForTeam(int team)
    {
        if (spawnPoints.ContainsKey(team))
        {
            return spawnPoints[team];
        }
        else
        {
            Debug.LogWarning("No spawn point registered for team: " + team);
            return Vector3.zero;
        }
    }
}