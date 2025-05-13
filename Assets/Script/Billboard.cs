using UnityEngine;

public class Billboard : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private Transform targetToFollow; // The object this will follow
    [SerializeField] private Vector3 offset = Vector3.zero; // Optional offset from the target

    [Header("Billboard Settings")]
    [SerializeField] private Camera mainCamera; // Camera to look at (defaults to main camera)

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        if (targetToFollow != null)
        {
            // Follow the target position with optional offset
            transform.position = targetToFollow.position + offset;
        }

        if (mainCamera != null)
        {
            // Make the object look at the camera (billboarding)
            transform.LookAt(mainCamera.transform);
            transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);
        }
    }
}
