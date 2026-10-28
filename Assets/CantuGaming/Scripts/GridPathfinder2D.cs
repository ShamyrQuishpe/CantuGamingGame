using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridPathfinder2D : MonoBehaviour
{
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap obstacleTilemap;

    private static readonly Vector3Int[] Directions =
    {
        new Vector3Int(1, 0, 0), new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0), new Vector3Int(0, -1, 0),
        new Vector3Int(1, 1, 0), new Vector3Int(1, -1, 0),
        new Vector3Int(-1, 1, 0), new Vector3Int(-1, -1, 0)
    };

    private readonly Queue<Vector3Int> frontier = new Queue<Vector3Int>();
    private readonly Dictionary<Vector3Int, Vector3Int> previous = new Dictionary<Vector3Int, Vector3Int>();

    public bool TryFindPath(Vector2 from, Vector2 to, List<Vector2> waypoints)
    {
        waypoints.Clear();
        if (floorTilemap == null || obstacleTilemap == null) return false;

        Vector3Int start = floorTilemap.WorldToCell(from);
        Vector3Int goal = floorTilemap.WorldToCell(to);
        if (!IsWalkable(start) || !IsWalkable(goal)) return false;
        if (start == goal) return true;

        // BFS: visita cada celda libre una sola vez hasta llegar al jugador.
        frontier.Clear();
        previous.Clear();
        frontier.Enqueue(start);
        previous.Add(start, start);

        while (frontier.Count > 0 && !previous.ContainsKey(goal))
        {
            Vector3Int current = frontier.Dequeue();
            foreach (Vector3Int step in Directions)
            {
                Vector3Int next = current + step;
                if (previous.ContainsKey(next) || !IsWalkable(next)) continue;

                // No permite cruzar en diagonal entre dos esquinas bloqueadas.
                if (step.x != 0 && step.y != 0 &&
                    (!IsWalkable(current + new Vector3Int(step.x, 0, 0)) ||
                     !IsWalkable(current + new Vector3Int(0, step.y, 0)))) continue;

                previous.Add(next, current);
                frontier.Enqueue(next);
            }
        }

        if (!previous.ContainsKey(goal)) return false;

        for (Vector3Int cell = goal; cell != start; cell = previous[cell])
            waypoints.Add(floorTilemap.GetCellCenterWorld(cell));

        waypoints.Reverse();
        return true;
    }

    private bool IsWalkable(Vector3Int cell)
    {
        return floorTilemap.HasTile(cell) && !obstacleTilemap.HasTile(cell);
    }
}
