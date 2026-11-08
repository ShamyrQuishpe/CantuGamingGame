using UnityEngine;

[DisallowMultipleComponent]
public class ShieldPickup2D : MonoBehaviour
{
    private ShieldPickupSpawner spawner;
    private PlayerShield2D playerShield;
    private bool collected;

    private void Awake()
    {
        transform.localScale = Vector3.one * 0.42f;

        SpriteRenderer face = gameObject.AddComponent<SpriteRenderer>();
        face.sprite = ShieldPrimitiveSprite.Square;
        face.color = Color.white;
        face.sortingOrder = 13;

        GameObject outline = new GameObject("Borde gris");
        outline.transform.SetParent(transform, false);
        outline.transform.localScale = Vector3.one * 1.18f;
        SpriteRenderer border = outline.AddComponent<SpriteRenderer>();
        border.sprite = ShieldPrimitiveSprite.Square;
        border.color = new Color(0.45f, 0.48f, 0.52f);
        border.sortingOrder = 12;

        BoxCollider2D pickupCollider = gameObject.AddComponent<BoxCollider2D>();
        pickupCollider.isTrigger = true;
        pickupCollider.size = Vector2.one * 1.8f;
    }

    public void Initialize(ShieldPickupSpawner owner, PlayerShield2D shield)
    {
        spawner = owner;
        playerShield = shield;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected || other == null) return;
        PlayerShield2D shield = other.GetComponentInParent<PlayerShield2D>();
        if (shield == null || shield != playerShield || !shield.TryActivate()) return;

        collected = true;
        spawner?.PickupCollected(this);
        Destroy(gameObject);
    }
}
