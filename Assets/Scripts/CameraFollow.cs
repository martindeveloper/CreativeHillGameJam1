using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
    public GameObject TargetToFollow;
    public float Damp = 0.2f;
    public Vector2 Offset = new Vector2(0.5f, 0.15f);

    private Vector3 MoveVelocity = Vector3.zero;
    private Transform TargetTransform;
    private Camera CameraComponent;
    private Transform CameraTransform;

    public void Start()
    {
        TargetTransform = TargetToFollow.GetComponent<Transform>();
        CameraTransform = GetComponent<Transform>();
        CameraComponent = GetComponent<Camera>();
    }

    public void FixedUpdate()
    {
        Vector3 targetPosition = TargetTransform.position;

        Vector3 point = CameraComponent.WorldToViewportPoint(targetPosition);
        Vector3 delta = targetPosition - CameraComponent.ViewportToWorldPoint(new Vector3(Offset.x, Offset.y, point.z));

        Vector3 destination = CameraTransform.position + delta;
        CameraTransform.position = Vector3.SmoothDamp(CameraTransform.position, destination, ref MoveVelocity, Damp);
     }
}
