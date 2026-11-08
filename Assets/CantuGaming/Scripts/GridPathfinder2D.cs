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

        Explore(start, goal, null);
        if (!previous.ContainsKey(goal)) return false;

        BuildPath(start, goal, waypoints);
        return true;
    }

    public bool TryFindFleePath(Vector2 from, Vector2 threat, List<Vector2> waypoints)
    {
        waypoints.Clear();
        if (floorTilemap == null || obstacleTilemap == null) return false;

        Vector3Int start = floorTilemap.WorldToCell(from);
        if (!IsWalkable(start)) return false;

        // La celda del jugador se trata como ocupada durante la huida.
        Vector3Int threatCell = floorTilemap.WorldToCell(threat);
        Explore(start, null, threatCell == start ? (Vector3Int?)null : threatCell);

        Vector3Int best = start;
        float bestDistance = ((Vector2)floorTilemap.GetCellCenterWorld(start) - threat).sqrMagnitude;
        foreach (Vector3Int cell in previous.Keys)
        {
            float distance = ((Vector2)floorTilemap.GetCellCenterWorld(cell) - threat).sqrMagnitude;
            if (distance <= bestDistance + 0.01f) continue;

            best = cell;
            bestDistance = distance;
        }

        BuildPath(start, best, waypoints);
        return true;
    }

    private void Explore(Vector3Int start, Vector3Int? goal, Vector3Int? blockedCell)
    {
        frontier.Clear();
        previous.Clear();
        frontier.Enqueue(start);
        previous.Add(start, start);

        while (frontier.Count > 0 && (!goal.HasValue || !previous.ContainsKey(goal.Value)))
        {
            Vector3Int current = frontier.Dequeue();
            foreach (Vector3Int step in Directions)
            {
                Vector3Int next = current + step;
                if (previous.ContainsKey(next) || next == blockedCell || !IsWalkable(next)) continue;

                // No permite cruzar en diagonal entre dos esquinas bloqueadas.
                if (step.x != 0 && step.y != 0)
                {
                    Vector3Int horizontal = current + new Vector3Int(step.x, 0, 0);
                    Vector3Int vertical = current + new Vector3Int(0, step.y, 0);
                    if (!IsWalkable(horizontal) || horizontal == blockedCell ||
                        !IsWalkable(vertical) || vertical == blockedCell) continue;
                }

                previous.Add(next, current);
                frontier.Enqueue(next);
            }
        }
    }

    private void BuildPath(Vector3Int start, Vector3Int goal, List<Vector2> waypoints)
    {
        for (Vector3Int cell = goal; cell != start; cell = previous[cell])
            waypoints.Add(floorTilemap.GetCellCenterWorld(cell));

        waypoints.Reverse();
    }

    private bool IsWalkable(Vector3Int cell)
    {
        return floorTilemap.HasTile(cell) && !obstacleTilemap.HasTile(cell);
    }
}
