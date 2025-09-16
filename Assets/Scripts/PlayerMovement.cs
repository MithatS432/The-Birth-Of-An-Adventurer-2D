using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sp;
    private Animator pa;

    public bool isGround;
    public float speed = 5f;
    public float jumpSpeed = 6f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();
        pa = GetComponent<Animator>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        }
    }

    private void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal");

        rb.linearVelocity = new Vector2(x * speed, rb.linearVelocity.y);

        pa.SetFloat("Speed", Mathf.Abs(x));

        if (x > 0.01f)
            sp.flipX = false;
        else if (x < -0.01f)
            sp.flipX = true;
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            isGround = true;
        }
    }
}
