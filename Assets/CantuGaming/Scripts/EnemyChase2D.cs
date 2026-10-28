using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyChase2D : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField, Min(0f)] private float speed = 2f;
    [SerializeField] private GridPathfinder2D navigation;
    [SerializeField, Min(0.1f)] private float repathInterval = 0.35f;
    [SerializeField, Min(0.01f)] private float waypointTolerance = 0.08f;

    private Rigidbody2D body;
    private readonly List<Vector2> waypoints = new List<Vector2>();
    private int waypointIndex;
    private float nextRepathTime;
    private bool hasPath;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (target == null)
        {
            SquareMover player = FindAnyObjectByType<SquareMover>();
            if (player != null) target = player.transform;
            else Debug.LogWarning("EnemyChase2D no encontro a Cantuna para perseguir.", this);
        }

        if (navigation == null) navigation = FindAnyObjectByType<GridPathfinder2D>();
        if (navigation == null) Debug.LogWarning("EnemyChase2D no encontro la navegacion; perseguira en linea recta.", this);
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            body.linearVelocity = Vector2.zero;
            return;
        }

        if (navigation == null)
        {
            MoveToward(target.position);
            return;
        }

        if (Time.time >= nextRepathTime)
        {
            hasPath = navigation.TryFindPath(body.position, target.position, waypoints);
            waypointIndex = 0;
            nextRepathTime = Time.time + repathInterval;
        }

        if (!hasPath)
        {
            body.linearVelocity = Vector2.zero;
            return;
        }

        if (waypoints.Count == 0)
        {
            MoveToward(target.position);
            return;
        }

        while (waypointIndex < waypoints.Count - 1 &&
               Vector2.Distance(body.position, waypoints[waypointIndex]) <= waypointTolerance)
            waypointIndex++;

        MoveToward(waypoints[waypointIndex]);
    }

    private void MoveToward(Vector2 destination)
    {
        Vector2 delta = destination - body.position;
        float distance = delta.magnitude;
        if (distance <= waypointTolerance)
        {
            body.linearVelocity = Vector2.zero;
            return;
        }

        body.linearVelocity = delta / distance * Mathf.Min(speed, distance / Time.fixedDeltaTime);
    }

    private void OnDisable()
    {
        if (body != null) body.linearVelocity = Vector2.zero;
    }
}
