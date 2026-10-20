using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SquareMover : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
<<<<<<< HEAD
=======
    [SerializeField] private float offsetAngle = -90f; // Ajusta según la orientación de tu sprite en reposo
>>>>>>> e5987c1 (Avance Proyecto)

    private Rigidbody2D body;
    private Vector2 direction;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float horizontal = 0f;
        float vertical = 0f;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) horizontal -= 1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) horizontal += 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) vertical -= 1f;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) vertical += 1f;

        direction = new Vector2(horizontal, vertical).normalized;
    }

    private void FixedUpdate()
    {
        body.linearVelocity = direction * speed;
<<<<<<< HEAD
=======

        // Solo rotamos si el personaje realmente se está moviendo
        if (direction != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            body.MoveRotation(targetAngle + offsetAngle);
        }
>>>>>>> e5987c1 (Avance Proyecto)
    }

    private void OnDisable()
    {
        if (body != null) body.linearVelocity = Vector2.zero;
    }
<<<<<<< HEAD
}
=======
}
>>>>>>> e5987c1 (Avance Proyecto)
