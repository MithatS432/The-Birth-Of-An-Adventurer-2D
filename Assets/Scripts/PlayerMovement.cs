using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sp;
    private Animator pa;

    public bool isGround;
    public float speed = 5f;
    public float jumpSpeed = 6f;

    public ParticleSystem firework;
    private float yRange = 404.23f;
    public TextMeshProUGUI wintext;
    public AudioClip winsound;
    float offset = 5f;

    public RopeLauncher ropeLauncher;
    private bool hasTriggeredFirework = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();
        pa = GetComponent<Animator>();
    }

    void Update()
    {
        if (transform.position.y > yRange && !hasTriggeredFirework)
        {
            FireWork();
            hasTriggeredFirework = true;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
            isGround = false;
        }
    }

    void FixedUpdate()
    {
        if (ropeLauncher == null || !ropeLauncher.ropeAttached)
        {
            float x = Input.GetAxis("Horizontal");
            rb.linearVelocity = new Vector2(x * speed, rb.linearVelocity.y);
            pa.SetFloat("Speed", Mathf.Abs(x));
            sp.flipX = x < -0.01f;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall2"))
            isGround = true;

        if (other.gameObject.CompareTag("Finish"))
            SceneManager.LoadScene(0);
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall2"))
            isGround = false;
    }

    void FireWork()
    {
        if (firework == null)
        {
            Debug.LogError("Firework particle system is not assigned!");
            return;
        }

        // Instantiate
        ParticleSystem fx = Instantiate(firework, transform.position, Quaternion.identity);
        fx.Play();

        // Coroutine ile manuel güncelleme
        StartCoroutine(UpdateParticleUnscaled(fx));

        // Zamanı durdur
        Time.timeScale = 0f;
        Debug.Log("Firework activated! Time stopped.");
    }

    IEnumerator UpdateParticleUnscaled(ParticleSystem fx)
    {
        while (fx != null && fx.IsAlive(true))
        {
            fx.Simulate(Time.unscaledDeltaTime, true, false);
            fx.Play();
            wintext.gameObject.SetActive(true);
            PlaySound(transform.position + new Vector3(offset, 0, 0));
            yield return null;
        }
    }
    void PlaySound(Vector3 position)
    {
        AudioSource.PlayClipAtPoint(winsound, position, 1f);
    }

}