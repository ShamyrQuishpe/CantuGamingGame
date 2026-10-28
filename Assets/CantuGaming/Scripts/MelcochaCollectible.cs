using UnityEngine;

public class MelcochaCollectible : MonoBehaviour
{
    [SerializeField] private int points = 200;

    private MelcochaSpawner spawner;
    private ScoreManager scoreManager;
    private bool collected;

    public void Initialize(MelcochaSpawner owner, ScoreManager score)
    {
        spawner = owner;
        scoreManager = score;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected || other.GetComponent<SquareMover>() == null) return;

        collected = true;
        scoreManager?.AddPoints(points);
        spawner?.SpawnReplacement();
        Destroy(gameObject);
    }
}
