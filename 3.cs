using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 8f, -14f);
    public float lookAtHeight = 4f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + target.TransformDirection(offset);
        transform.position = desiredPosition;

        Vector3 lookTarget = target.position + Vector3.up * lookAtHeight;
        transform.LookAt(lookTarget);
    }
}