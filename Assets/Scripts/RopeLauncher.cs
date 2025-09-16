using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RopeLauncher : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public LayerMask groundLayer;
    public float ropeSpeed = 20f;
    public float pullForce = 500f;       
    public float maxRopeDistance = 10f;

    [Range(2, 50)]
    public int lineSegments = 5;        

    [HideInInspector]
    public bool ropeAttached = false;

    private Vector2 ropeTarget;
    private Vector2 ropeCurrent;
    private bool ropeFired = false;

    private Rigidbody2D rb;
    private AudioSource audioSource;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        lineRenderer.positionCount = 0;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            FireRope();
            if (audioSource != null) audioSource.Play();
        }

        if (Input.GetMouseButtonDown(1))
            ReleaseRope();

        if (ropeFired && !ropeAttached)
            MoveRopeHead();

        if (ropeAttached)
            PullCharacter();

        UpdateRopeVisual();
    }

    void FireRope()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mousePos - (Vector2)transform.position).normalized;

        ropeCurrent = (Vector2)transform.position;

        RaycastHit2D hit = Physics2D.Raycast(ropeCurrent, dir, maxRopeDistance, groundLayer);

        if (hit.collider != null)
        {
            ropeTarget = hit.point;
            ropeFired = true;
            ropeAttached = false;
            lineRenderer.positionCount = lineSegments;
        }
        else
        {
            ropeFired = false;
            ropeAttached = false;
            lineRenderer.positionCount = 0;
        }
    }

    void MoveRopeHead()
    {
        ropeCurrent = Vector2.MoveTowards(ropeCurrent, ropeTarget, ropeSpeed * Time.deltaTime);

        if (Vector2.Distance(ropeCurrent, ropeTarget) < 0.1f)
        {
            ropeAttached = true;
            ropeFired = false;
        }
    }

    void PullCharacter()
    {
        Vector2 direction = (ropeTarget - (Vector2)transform.position).normalized;
        rb.AddForce(direction * pullForce * Time.fixedDeltaTime, ForceMode2D.Force);

        if (Vector2.Distance(transform.position, ropeTarget) < 0.3f)
        {
            ReleaseRope();
        }
    }

    void ReleaseRope()
    {
        ropeAttached = false;
        ropeFired = false;
        lineRenderer.positionCount = 0;
    }

    void UpdateRopeVisual()
    {
        if (lineRenderer.positionCount < 2) return;

        Vector3 start = transform.position;
        Vector3 end = ropeAttached || ropeFired ? (Vector3)ropeCurrent : transform.position;

        for (int i = 0; i < lineSegments; i++)
        {
            float t = i / (float)(lineSegments - 1);
            lineRenderer.SetPosition(i, Vector3.Lerp(start, end, t));
        }
    }
}
