using UnityEngine;

public class MelcochaCollectible : MonoBehaviour
{
    private MelcochaSpawner spawner;
    private bool collected;

    public void Initialize(MelcochaSpawner owner)
    {
        spawner = owner;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected || other.GetComponent<SquareMover>() == null) return;

        collected = true;
        spawner?.SpawnReplacement();
        Destroy(gameObject);
    }
}
