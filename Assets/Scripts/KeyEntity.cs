using UnityEngine;
using System.Collections;

public class KeyEntity : MonoBehaviour {
    public float FloatingSpeed = 2.5f;
    public float FloatingAmplitude = 2.0f;

    public DoorEntity AssociatedDoor;

    private Transform TransformComponent;

    public void Start()
    {
        TransformComponent = GetComponent<Transform>();
    }

    public void Update()
    {
        Vector3 position = TransformComponent.position;

        position.y = position.y + (FloatingAmplitude / 100.0f) * Mathf.Sin(FloatingSpeed * Time.time);

        TransformComponent.position = position;
    }

    public void Pickup()
    {
        // Open door
        AssociatedDoor.Unlock();

        // Destroy self
        Destroy(gameObject);
    }
}
