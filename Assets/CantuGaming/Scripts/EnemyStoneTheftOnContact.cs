using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemyStoneTheftOnContact : MonoBehaviour
{
    [SerializeField] private StoneFollowAnchor stone;
    [SerializeField] private Transform playerAnchor;
    [SerializeField] private Transform enemyAnchor;

    private readonly HashSet<Collider2D> playerContacts = new HashSet<Collider2D>();
    private bool transferredDuringContact;

    public bool IsPlayerHoldingStone => stone != null && playerAnchor != null && stone.Anchor == playerAnchor;
    public bool IsEnemyHoldingStone => stone != null && enemyAnchor != null && stone.Anchor == enemyAnchor;

    private void OnCollisionEnter2D(Collision2D collision) { TryTransferOnContact(collision.collider); }
    private void OnCollisionStay2D(Collision2D collision) { TryTransferOnContact(collision.collider); }
    private void OnCollisionExit2D(Collision2D collision) { EndContact(collision.collider); }
    private void OnTriggerEnter2D(Collider2D other) { TryTransferOnContact(other); }
    private void OnTriggerStay2D(Collider2D other) { TryTransferOnContact(other); }
    private void OnTriggerExit2D(Collider2D other) { EndContact(other); }

    private void TryTransferOnContact(Collider2D other)
    {
        if (!isActiveAndEnabled || other == null || other.GetComponentInParent<SquareMover>() == null) return;
        playerContacts.Add(other);
        if (transferredDuringContact || stone == null || playerAnchor == null || enemyAnchor == null) return;

        // El controlador de la piedra decide si el cambio está permitido y aplica la protección.
        bool transferred = stone.TryTransfer(playerAnchor, enemyAnchor);
        if (!transferred) transferred = stone.TryTransfer(enemyAnchor, playerAnchor);
        if (transferred) transferredDuringContact = true;
    }

    private void EndContact(Collider2D other)
    {
        playerContacts.Remove(other);
        if (playerContacts.Count == 0) transferredDuringContact = false;
    }

    private void OnDisable()
    {
        playerContacts.Clear();
        transferredDuringContact = false;
    }
}
