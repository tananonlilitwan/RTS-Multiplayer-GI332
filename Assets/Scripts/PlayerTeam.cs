using Unity.Netcode;
using UnityEngine;

public class PlayerTeam : NetworkBehaviour
{
    public NetworkVariable<int> Team = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            SetTeamServerRpc(NetworkManagerUI.selectedTeam);
        }
    }

    [ServerRpc]
    void SetTeamServerRpc(int team)
    {
        Team.Value = team;
    }
}