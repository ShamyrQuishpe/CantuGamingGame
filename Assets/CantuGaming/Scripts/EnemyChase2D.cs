using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyChase2D : MonoBehaviour
{
    [Header("Configuración Principal")]
    [SerializeField] private Transform target;
    [SerializeField, Min(0f)] private float speed = 2f;
    
    [Header("Gráficos (Asignar a Diablo aquí)")]
    [SerializeField] private Transform graficos; 
    [SerializeField] private float ajusteDeAngulo = -90f; // Cambia esto en el inspector si el modelo mira de lado

    [Header("Navegación")]
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
        }

        if (navigation == null) navigation = FindAnyObjectByType<GridPathfinder2D>();
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

        if (!hasPath || waypoints.Count == 0)
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

        Vector2 direction = delta / distance;
        body.linearVelocity = direction * Mathf.Min(speed, distance / Time.fixedDeltaTime);

        // --- ROTACIÓN TOP-DOWN (360 GRADOS) ---
        if (graficos != null && direction.sqrMagnitude > 0.01f)
        {
            // Calcula el ángulo en base a la dirección de movimiento
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            
            // Rota solo el modelo visual en el eje Z (rotación en 2D)
            graficos.rotation = Quaternion.Euler(0, 0, angle + ajusteDeAngulo);
        }
    }

    private void OnDisable()
    {
        if (body != null) body.linearVelocity = Vector2.zero;
    }
}