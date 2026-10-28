using UnityEngine;

public class EnemyContactGameOver : MonoBehaviour
{
    [SerializeField] private GameOverManager gameOverManager;

    private void Awake()
    {
        if (gameOverManager == null) gameOverManager = FindAnyObjectByType<GameOverManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryEndGame(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryEndGame(other);
    }

    private void TryEndGame(Collider2D other)
    {
        if (other.GetComponentInParent<SquareMover>() == null) return;

        if (gameOverManager == null) gameOverManager = FindAnyObjectByType<GameOverManager>();
        gameOverManager?.TriggerGameOver();
    }
}
