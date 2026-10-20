using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SquareMover : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

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
    }

    private void OnDisable()
    {
        if (body != null) body.linearVelocity = Vector2.zero;
    }
}
