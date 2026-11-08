using UnityEngine;

[DisallowMultipleComponent]
public class StoneFollowAnchor : MonoBehaviour
{
    [SerializeField] private Transform anchor;

    public Transform Anchor => anchor;

    public void SetAnchor(Transform newAnchor)
    {
        anchor = newAnchor;
        FollowAnchor();
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
