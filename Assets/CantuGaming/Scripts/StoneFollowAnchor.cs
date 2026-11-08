using System;
using UnityEngine;

[DisallowMultipleComponent]
public class StoneFollowAnchor : MonoBehaviour
{
    [SerializeField] private Transform anchor;
    [SerializeField, Min(0f)] private float transferProtectionSeconds = 0.8f;

    private float nextAllowedTransferTime;

    // El ancla actual es la única fuente de verdad sobre quién lleva la piedra.
    public Transform Anchor => anchor;
    public event Action<Transform> StoneTransferred;

    public void SetAnchor(Transform newAnchor)
    {
        anchor = newAnchor;
        FollowAnchor();
    }

    public bool TryTransfer(Transform expectedAnchor, Transform newAnchor)
    {
        if (expectedAnchor == null || anchor != expectedAnchor || newAnchor == null ||
            newAnchor == anchor || Time.time < nextAllowedTransferTime) return false;

        SetAnchor(newAnchor);
        nextAllowedTransferTime = Time.time + transferProtectionSeconds;
        StoneTransferred?.Invoke(newAnchor);
        return true;
    }

    private void LateUpdate()
    {
        FollowAnchor();
    }

    private void FollowAnchor()
    {
        if (anchor == null) return;

        transform.SetPositionAndRotation(anchor.position, anchor.rotation);
    }
}
