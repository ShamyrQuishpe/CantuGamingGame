using UnityEngine;

public class MelcochaSpawner : MonoBehaviour
{
    [SerializeField] private MelcochaCollectible melcochaPrefab;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private int initialCount = 3;
    [SerializeField] private Vector2Int minCell = new Vector2Int(-14, -7);
    [SerializeField] private Vector2Int maxCell = new Vector2Int(13, 6);

    private void Start()
    {
        if (melcochaPrefab == null)
        {
            Debug.LogError("MelcochaSpawner necesita un prefab de melcocha.", this);
            return;
        }

        for (int i = 0; i < initialCount; i++) SpawnOne();
    }

    public void SpawnReplacement()
    {
        SpawnOne();
    }

    private void SpawnOne()
    {
        if (melcochaPrefab == null) return;

        for (int attempt = 0; attempt < 100; attempt++)
        {
            int x = Random.Range(minCell.x, maxCell.x + 1);
            int y = Random.Range(minCell.y, maxCell.y + 1);
            Vector2 position = new Vector2(x + 0.5f, y + 0.5f);

            // Evita muros, jardineras, al jugador y otras melcochas.
            if (Physics2D.OverlapCircle(position, 0.45f) != null) continue;

            MelcochaCollectible instance = Instantiate(melcochaPrefab, position, Quaternion.identity);
            instance.Initialize(this, scoreManager);
            return;
        }

        Debug.LogWarning("No se encontro un espacio libre para generar una melcocha.", this);
    }
}
