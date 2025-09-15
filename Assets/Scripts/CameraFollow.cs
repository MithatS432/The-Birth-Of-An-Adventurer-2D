using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    Vector3 offset; // The target the camera will follow
    void Start()
    {
        offset = transform.position - player.position;
        offset.z = -10;
    }

    void Update()
    {
        transform.position = player.position + offset;
    }
}
