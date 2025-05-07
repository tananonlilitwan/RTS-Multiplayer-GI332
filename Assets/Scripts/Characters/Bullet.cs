using Unity.Netcode;
using UnityEngine;


public enum Team
{
    A = 0,
    B = 1
}

public class Bullet : NetworkBehaviour
{
    [SerializeField] public float speed = 10f;
    [SerializeField] public int damage = 10;

    private NetworkVariable<Vector2> direction = new NetworkVariable<Vector2>();
    private NetworkVariable<int> team = new NetworkVariable<int>(); // team stored as int

    private bool hasRotated = false;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Destroy(gameObject, 5f); // Cleanup bullet after 5 seconds
        }
    }

    public void Initialize(Vector2 direction, Team team)
    {
        this.direction.Value = direction;
        this.team.Value = (int)team; // convert enum to int before storing
    }

    private void Update()
    {
        if (!IsSpawned) return;

        if (!hasRotated && direction.Value != Vector2.zero)
        {
            RotateTowardsDirection();
            hasRotated = true;
        }

        transform.Translate(direction.Value * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) return;

        int bulletTeam = team.Value;

        if (other.TryGetComponent<Unit>(out var unit))
        {
            if ((int)unit.team != bulletTeam)
            {
                unit.TakeDamage(damage);
                DespawnSelf();
            }
        }
        else if (other.TryGetComponent<Base>(out var baseObj))
        {
            if ((int)baseObj.team != bulletTeam)
            {
                baseObj.TakeDamage(damage);
                DespawnSelf();
            }
        }
        else if (other.TryGetComponent<Building>(out var building))
        {
            if ((int)building.team != bulletTeam)
            {
                building.TakeDamage(damage);
                DespawnSelf();
            }
        }
        else if (other.TryGetComponent<ConstructionWorker>(out var worker))
        {
            if ((int)worker.team != bulletTeam)
            {
                worker.TakeDamage(damage);
                DespawnSelf();
            }
        }
    }

    private void DespawnSelf()
    {
        if (IsServer)
        {
            GetComponent<NetworkObject>().Despawn(true);
        }
    }

    private void RotateTowardsDirection()
    {
        float angle = Mathf.Atan2(direction.Value.y, direction.Value.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
