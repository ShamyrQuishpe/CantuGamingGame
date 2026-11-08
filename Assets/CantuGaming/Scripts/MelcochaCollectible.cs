using UnityEngine;

public class MelcochaCollectible : MonoBehaviour
{
    [SerializeField] private int points = 200;

    private MelcochaSpawner spawner;
    private ScoreManager scoreManager;
    private GameAudio gameAudio;
    private bool collected;

    public void Initialize(MelcochaSpawner owner, ScoreManager score, GameAudio audio)
    {
        spawner = owner;
        scoreManager = score;
        gameAudio = audio;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected || other.GetComponent<SquareMover>() == null) return;

        collected = true;
        scoreManager?.AddPoints(points);
        gameAudio?.PlayMelcocha();
        spawner?.SpawnReplacement();
        Destroy(gameObject);
    }
}
