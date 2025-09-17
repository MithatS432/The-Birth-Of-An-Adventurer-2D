using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class RopeLauncher : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public LayerMask groundLayer;
    public LayerMask obstacleLayer;
    public float ropeSpeed = 20f;
    public float pullForce = 500f;
    public float maxRopeDistance = 10f;
    public float disableDuration = 4f;

    [Range(2, 50)]
    public int lineSegments = 5;

    [HideInInspector]
    public bool ropeAttached = false;

    private Vector2 ropeTarget;
    private Vector2 ropeCurrent;
    private bool ropeFired = false;
    private bool isPassingThroughObstacle = false;

    private Rigidbody2D rb;
    private AudioSource audioSource;
    private Collider2D playerCollider;
    private Collider2D currentObstacleCollider;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        playerCollider = GetComponent<Collider2D>();
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

        RaycastHit2D hit = Physics2D.Raycast(ropeCurrent, dir, maxRopeDistance, groundLayer | obstacleLayer);

        if (hit.collider != null)
        {
            ropeTarget = hit.point;
            ropeFired = true;
            ropeAttached = false;
            lineRenderer.positionCount = lineSegments;

            // Engel mi kontrol et
            if (((1 << hit.collider.gameObject.layer) & obstacleLayer) != 0)
            {
                currentObstacleCollider = hit.collider;
                isPassingThroughObstacle = true;
            }
            else
            {
                isPassingThroughObstacle = false;
                currentObstacleCollider = null;
            }
        }
        else
        {
            ropeFired = false;
            ropeAttached = false;
            lineRenderer.positionCount = 0;
            isPassingThroughObstacle = false;
            currentObstacleCollider = null;
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
        Vector2 currentPos = rb.position;
        Vector2 direction = (ropeTarget - currentPos).normalized;
        float distance = Vector2.Distance(currentPos, ropeTarget);

        // Engele yaklaşıyorsa collider'ı devre dışı bırak
        if (isPassingThroughObstacle && currentObstacleCollider != null &&
            distance < 2f && currentObstacleCollider.enabled)
        {
            StartCoroutine(DisableObstacleTemporarily(currentObstacleCollider, disableDuration));
        }

        if (distance < 0.5f)
        {
            ReleaseRope();
            return;
        }

        float pullMultiplier = Mathf.Lerp(1f, 5f, 1 - (distance / maxRopeDistance));
        Vector2 force = direction * pullForce * pullMultiplier * Time.deltaTime;

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.AddForce(force, ForceMode2D.Force);
    }

    IEnumerator DisableObstacleTemporarily(Collider2D obstacleCollider, float duration)
    {
        obstacleCollider.enabled = false;
        yield return new WaitForSeconds(duration);

        if (obstacleCollider != null)
        {
            obstacleCollider.enabled = true;
        }

        currentObstacleCollider = null;
        isPassingThroughObstacle = false;
    }

    void ReleaseRope()
    {
        ropeAttached = false;
        ropeFired = false;
        lineRenderer.positionCount = 0;
        isPassingThroughObstacle = false;

        rb.bodyType = RigidbodyType2D.Dynamic;
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

    void OnDestroy()
    {
        // Script yok olurken collider'ları tekrar aktif et
        if (currentObstacleCollider != null && !currentObstacleCollider.enabled)
        {
            currentObstacleCollider.enabled = true;
        }
    }

    void OnDisable()
    {
        // Script devre dışı kalırsa collider'ları tekrar aktif et
        if (currentObstacleCollider != null && !currentObstacleCollider.enabled)
        {
            currentObstacleCollider.enabled = true;
        }
    }
}