using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyChase2D : MonoBehaviour
{
    [Header("Configuración Principal")]
    [SerializeField] private Transform target;
    [SerializeField, Min(0f)] private float speed = 2f;
    [SerializeField, Min(0f)] private float fleeSpeed = 5f;

    [Header("Gráficos (Asignar a Diablo aquí)")]
    [SerializeField] private Transform graficos;
    [SerializeField] private float ajusteDeAngulo = -90f;

    [Header("Navegación")]
    [SerializeField] private GridPathfinder2D navigation;
    [SerializeField, Min(0.1f)] private float repathInterval = 0.35f;
    [SerializeField, Min(0.01f)] private float waypointTolerance = 0.08f;
    [SerializeField, Min(0.1f)] private float fleeRepathInterval = 1f;
    [SerializeField, Min(0.1f)] private float fleeDangerDistance = 4f;
    [SerializeField, Min(0.1f)] private float fleeRetargetDistance = 1.5f;

    private enum MovementMode { Idle, Chase, Flee }

    private Rigidbody2D body;
    private EnemyStoneTheftOnContact stoneTheft;
    private readonly List<Vector2> waypoints = new List<Vector2>();
    private int waypointIndex;
    private float nextRepathTime;
    private Vector2 threatAtLastPlan;
    private bool hasPath;
    private MovementMode lastMode = MovementMode.Idle;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        stoneTheft = GetComponent<EnemyStoneTheftOnContact>();
    }

    private void Start()
    {
        if (target == null)
        {
            SquareMover player = FindAnyObjectByType<SquareMover>();
            if (player != null) target = player.transform;
        }

        if (navigation == null) navigation = FindAnyObjectByType<GridPathfinder2D>();
    }

    private void FixedUpdate()
    {
        MovementMode mode = GetMovementMode();
        if (mode != lastMode)
        {
            waypoints.Clear();
            waypointIndex = 0;
            hasPath = false;
            nextRepathTime = 0f;
            lastMode = mode;
        }

        if (target == null || mode == MovementMode.Idle)
        {
            body.linearVelocity = Vector2.zero;
            return;
        }

        if (mode == MovementMode.Flee)
            FollowFleePath();
        else
            FollowChasePath();
    }

    private MovementMode GetMovementMode()
    {
        if (stoneTheft == null) return MovementMode.Chase;
        if (stoneTheft.IsEnemyHoldingStone) return MovementMode.Flee;
        if (stoneTheft.IsPlayerHoldingStone) return MovementMode.Chase;
        return MovementMode.Idle;
    }

    private void FollowChasePath()
    {
        if (navigation == null)
        {
            MoveToward(target.position, speed);
            return;
        }

        if (Time.time >= nextRepathTime)
        {
            hasPath = navigation.TryFindPath(body.position, target.position, waypoints);
            waypointIndex = 0;
            nextRepathTime = Time.time + repathInterval;
        }

        if (!hasPath || waypoints.Count == 0)
        {
            MoveToward(target.position, speed);
            return;
        }

        AdvanceWaypoint();
        MoveToward(waypoints[waypointIndex], speed);
    }

    private void FollowFleePath()
    {
        // Sin ruta transitable no se mueve en línea recta a través del escenario.
        if (navigation == null)
        {
            body.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 threat = target.position;
        bool reachedGoal = waypoints.Count == 0 ||
            Vector2.Distance(body.position, waypoints[waypoints.Count - 1]) <= waypointTolerance;
        bool threatNearby = Vector2.Distance(body.position, threat) <= fleeDangerDistance;
        bool threatMoved = Vector2.Distance(threat, threatAtLastPlan) >= fleeRetargetDistance;

        if (Time.time >= nextRepathTime &&
            (!hasPath || (threatNearby && (reachedGoal || threatMoved))))
        {
            hasPath = navigation.TryFindFleePath(body.position, threat, waypoints);
            waypointIndex = 0;
            threatAtLastPlan = threat;
            nextRepathTime = Time.time + fleeRepathInterval;
            reachedGoal = waypoints.Count == 0;
        }

        if (!hasPath || reachedGoal)
        {
            body.linearVelocity = Vector2.zero;
            return;
        }

        AdvanceWaypoint();
        MoveToward(waypoints[waypointIndex], fleeSpeed);
    }

    private void AdvanceWaypoint()
    {
        while (waypointIndex < waypoints.Count - 1 &&
               Vector2.Distance(body.position, waypoints[waypointIndex]) <= waypointTolerance)
            waypointIndex++;
    }

    private void MoveToward(Vector2 destination, float moveSpeed)
    {
        Vector2 delta = destination - body.position;
        float distance = delta.magnitude;

        if (distance <= waypointTolerance)
        {
            body.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 direction = delta / distance;
        body.linearVelocity = direction * Mathf.Min(moveSpeed, distance / Time.fixedDeltaTime);

        // Rota solo el modelo visual en el eje Z (rotación en 2D).
        if (graficos != null && direction.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            graficos.rotation = Quaternion.Euler(0, 0, angle + ajusteDeAngulo);
        }
    }

    private void OnDisable()
    {
        if (body != null) body.linearVelocity = Vector2.zero;
    }
}
