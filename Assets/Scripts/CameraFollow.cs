using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    Vector3 offset;
    void Start()
    {
        offset = transform.position - player.position;
        offset.z = -10;
    }

    private void LateUpdate()
    {
        transform.position = player.position + offset;
    }
}
