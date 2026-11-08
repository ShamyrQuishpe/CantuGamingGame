using System;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SquareMover))]
public class PlayerShield2D : MonoBehaviour
{
    [SerializeField, Min(0.2f)] private float ringRadius = 0.8f;
    [SerializeField, Min(0.01f)] private float ringWidth = 0.07f;

    private const float SparkDuration = 0.45f;
    private GameObject ring;
    private GameObject sparkRoot;
    private SpriteRenderer[] sparks;
    private Vector2[] sparkDirections;
    private Material ringMaterial;
    private float sparkTime;

    public bool HasShield { get; private set; }
    public event Action ShieldConsumed;

    private void Awake()
    {
        CreateRing();
        CreateSparks();
    }

    public bool TryActivate()
    {
        if (HasShield) return false;

        HasShield = true;
        ring.SetActive(true);
        BurstSparks();
        return true;
    }

    public bool TryAbsorbHit()
    {
        if (!HasShield) return false;

        HasShield = false;
        ring.SetActive(false);
        BurstSparks();
        ShieldConsumed?.Invoke();
        return true;
    }

    private void CreateRing()
    {
        ring = new GameObject("Circulo del escudo");
        ring.transform.SetParent(transform, false);
        // Cantuña está escalado en la escena; el círculo conserva tamaño de mundo.
        ring.transform.localScale = UnscaledChildSize();
        LineRenderer line = ring.AddComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.loop = true;
        line.positionCount = 48;
        line.startWidth = ringWidth;
        line.endWidth = ringWidth;
        line.startColor = new Color(0.6f, 0.95f, 1f, 0.95f);
        line.endColor = line.startColor;
        line.sortingOrder = 20;
        Shader shader = Shader.Find("Sprites/Default");
        if (shader != null)
        {
            ringMaterial = new Material(shader);
            line.material = ringMaterial;
        }

        for (int i = 0; i < line.positionCount; i++)
        {
            float angle = i * Mathf.PI * 2f / line.positionCount;
            line.SetPosition(i, new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * ringRadius);
        }
        ring.SetActive(false);
    }

    private void CreateSparks()
    {
        sparkRoot = new GameObject("Chispas del escudo");
        sparkRoot.transform.SetParent(transform, false);
        sparkRoot.transform.localScale = UnscaledChildSize();
        sparks = new SpriteRenderer[12];
        sparkDirections = new Vector2[sparks.Length];

        for (int i = 0; i < sparks.Length; i++)
        {
            float angle = i * Mathf.PI * 2f / sparks.Length;
            sparkDirections[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            GameObject spark = new GameObject("Chispa");
            spark.transform.SetParent(sparkRoot.transform, false);
            spark.transform.localScale = Vector3.one * 0.1f;
            sparks[i] = spark.AddComponent<SpriteRenderer>();
            sparks[i].sprite = ShieldPrimitiveSprite.Square;
            sparks[i].sortingOrder = 21;
        }
        sparkRoot.SetActive(false);
    }

    private Vector3 UnscaledChildSize()
    {
        Vector3 scale = transform.lossyScale;
        return new Vector3(1f / scale.x, 1f / scale.y, 1f);
    }

    private void BurstSparks()
    {
        sparkTime = SparkDuration;
        sparkRoot.SetActive(true);
        UpdateSparks(0f);
    }

    private void Update()
    {
        if (sparkTime <= 0f) return;

        sparkTime = Mathf.Max(0f, sparkTime - Time.deltaTime);
        UpdateSparks(1f - sparkTime / SparkDuration);
        if (sparkTime == 0f) sparkRoot.SetActive(false);
    }

    private void UpdateSparks(float progress)
    {
        for (int i = 0; i < sparks.Length; i++)
        {
            float distance = ringRadius * 0.65f + 1.3f * progress;
            sparks[i].transform.localPosition = sparkDirections[i] * distance;
            sparks[i].color = new Color(1f, 0.95f, 0.6f, 1f - progress);
        }
    }

    private void OnDestroy()
    {
        if (ringMaterial != null) Destroy(ringMaterial);
    }
}

internal static class ShieldPrimitiveSprite
{
    private static Sprite square;

    public static Sprite Square
    {
        get
        {
            if (square == null)
            {
                Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                texture.filterMode = FilterMode.Point;
                texture.SetPixels(new[] { Color.white, Color.white, Color.white, Color.white });
                texture.Apply();
                square = Sprite.Create(texture, new Rect(0f, 0f, 2f, 2f), new Vector2(0.5f, 0.5f), 2f);
                square.name = "Cuadrado blanco del escudo";
            }
            return square;
        }
    }
}
