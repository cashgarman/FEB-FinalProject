using UnityEngine;

public class Artifact : MonoBehaviour
{
    private Rigidbody _rigidBody;

    public bool Stashed { get; set; }
    public bool PickedUp { get; set; }
    public Vector3 HomePosition { get; set; }

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        HomePosition = transform.position;
    }

    public void OnPickedUp(Transform holdPoint)
    {
        // Don't pickup already stashed artifacts
        if (Stashed)
            return;

        PickedUp = true;
        _rigidBody.useGravity = false;
        _rigidBody.isKinematic = true;
        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.rotation = holdPoint.rotation;
    }

    public void OnDropped()
    {
        PickedUp = false;
        _rigidBody.useGravity = true;
        _rigidBody.isKinematic = false;
        transform.SetParent(null);
    }
}
