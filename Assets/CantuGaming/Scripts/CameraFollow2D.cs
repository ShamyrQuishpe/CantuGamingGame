using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [SerializeField] private Transform target;

    private float cameraZ;

    public Transform Target => target;

    public void SetTarget(Transform followTarget)
    {
        target = followTarget;
    }

    private void Awake()
    {
        cameraZ = transform.position.z;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        transform.position = new Vector3(target.position.x, target.position.y, cameraZ);
    }
}
