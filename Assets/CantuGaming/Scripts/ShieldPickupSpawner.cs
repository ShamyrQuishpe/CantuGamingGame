using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
public class ShieldPickupSpawner : MonoBehaviour
{
    [SerializeField] private PlayerShield2D playerShield;
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap obstacleTilemap;
    [SerializeField, Min(1f)] private float spawnRadius = 5f;
    [SerializeField] private Vector2Int minCell = new Vector2Int(-14, -7);
    [SerializeField] private Vector2Int maxCell = new Vector2Int(13, 6);

    private ShieldPickup2D activePickup;

    private void OnEnable()
    {
        if (playerShield != null) playerShield.ShieldConsumed += SpawnOne;
    }

    private void Start()
    {
        SpawnOne();
    }

    private void OnDisable()
    {
        if (playerShield != null) playerShield.ShieldConsumed -= SpawnOne;
    }

    public void PickupCollected(ShieldPickup2D pickup)
    {
        if (activePickup == pickup) activePickup = null;
    }

    private void SpawnOne()
    {
        if (activePickup != null || playerShield == null || playerShield.HasShield ||
            floorTilemap == null || obstacleTilemap == null) return;

        // Primero busca cerca de Cantuña para que el cubito sea fácil de encontrar.
        if (!TryChooseFreePosition(spawnRadius, out Vector3 chosen) &&
            !TryChooseFreePosition(float.PositiveInfinity, out chosen))
        {
            Debug.LogWarning("No hay una celda libre para el escudo.", this);
            return;
        }

        GameObject cube = new GameObject("Cubito blanco (escudo)");
        cube.transform.position = chosen;
        activePickup = cube.AddComponent<ShieldPickup2D>();
        activePickup.Initialize(this, playerShield);
    }

    private bool TryChooseFreePosition(float maxDistance, out Vector3 chosen)
    {
        chosen = Vector3.zero;
        int freeCount = 0;
        for (int x = minCell.x; x <= maxCell.x; x++)
        {
            for (int y = minCell.y; y <= maxCell.y; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);
                if (!floorTilemap.HasTile(cell) || obstacleTilemap.HasTile(cell)) continue;
                Vector3 center = floorTilemap.GetCellCenterWorld(cell);
                if (Vector2.Distance(center, playerShield.transform.position) > maxDistance ||
                    Physics2D.OverlapCircle(center, 0.45f) != null) continue;

                freeCount++;
                if (Random.Range(0, freeCount) == 0) chosen = center;
            }
        }
        return freeCount > 0;
    }
}
