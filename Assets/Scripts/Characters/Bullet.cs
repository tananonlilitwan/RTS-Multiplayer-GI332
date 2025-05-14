using Unity.Netcode;
using UnityEngine;

public enum Team
{
    A = 0,
    B = 1
}

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : NetworkBehaviour
{
    [SerializeField] public float speed = 10f;
    [SerializeField] public int damage = 10;

    private NetworkVariable<Vector2> direction = new NetworkVariable<Vector2>();
    private NetworkVariable<int> team = new NetworkVariable<int>();

    private Rigidbody2D rb;

    public override void OnNetworkSpawn()
    {
        rb = GetComponent<Rigidbody2D>();

        if (IsServer)
        {
            // ตั้งความเร็วให้ Rigidbody2D พุ่งไปในทิศที่กำหนด
            rb.velocity = direction.Value.normalized * speed;

            // ตั้งเวลาให้กระสุนหายหลัง 5 วินาที
            Destroy(gameObject, 5f);
        }
    }

    public void Initialize(Vector2 dir, Team team)
    {
        direction.Value = dir.normalized;
        this.team.Value = (int)team;

        // หมุนกระสุนให้หัวหันไปทิศยิง
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnTriggerEnter2D(Collider2D other)
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
                building.TakeDamageServerRpc(damage);
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
}
