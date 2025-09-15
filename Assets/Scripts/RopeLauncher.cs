using UnityEngine;

public class RopeLauncher : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public LayerMask groundLayer;
    public float ropeSpeed = 20f;     // Halatın fırlatma hızı
    public float pullSpeed = 10f;     // Karakterin çekilme hızı

    private Vector2 ropeTarget;
    private Vector2 ropeCurrent;
    private bool ropeFired = false;
    private bool ropeAttached = false;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        lineRenderer.positionCount = 0;
    }

    void Update()
    {
        // Sol tık -> halat fırlat
        if (Input.GetMouseButtonDown(0))
        {
            FireRope();
        }

        // Sağ tık -> halatı bırak
        if (Input.GetMouseButtonDown(1))
        {
            ReleaseRope();
        }

        // Halat fırlatıldıysa ucu hareket ettir
        if (ropeFired && !ropeAttached)
        {
            MoveRopeHead();
        }

        // Halat bağlıysa karakteri çek
        if (ropeAttached)
        {
            PullCharacter();
        }

        UpdateRopeVisual();
    }

    void FireRope()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        ropeTarget = mousePos;
        ropeCurrent = transform.position; // Halat karakterden başlar
        ropeFired = true;
        ropeAttached = false;
        lineRenderer.positionCount = 2;
    }

    void MoveRopeHead()
    {
        // Halat ucunu hedefe doğru hareket ettir
        ropeCurrent = Vector2.MoveTowards(ropeCurrent, ropeTarget, ropeSpeed * Time.deltaTime);

        // Hedefe ulaştıysa halatı bağlı kabul et
        if (Vector2.Distance(ropeCurrent, ropeTarget) < 0.1f)
        {
            ropeAttached = true;
            ropeFired = false;
        }
    }

    void PullCharacter()
    {
        Vector2 direction = (ropeTarget - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * pullSpeed;

        if (Vector2.Distance(transform.position, ropeTarget) < 0.1f)
        {
            rb.linearVelocity = Vector2.zero;
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
        if (lineRenderer.positionCount > 0)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, ropeFired || ropeAttached ? (Vector3)ropeCurrent : transform.position);
        }
    }
}
