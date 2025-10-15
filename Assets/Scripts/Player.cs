using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public GameObject hitPrefab;
    public GameObject gameOver;
    public GameObject gameClear;
    public Slider slider;
    public Text HPText;
    public int maxHP;
    public int maxJumpCount = 2;

    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded;
    private bool isOver;
    private int jumpCount;
    private int HP;
    private bool isFalling;

    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;

    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        isOver = false;
        isFalling = false;
        isGrounded = true;
        HP = maxHP;
        jumpCount = maxJumpCount;

        HPText.text = $"HP: {maxHP}";
    }

    void Update()
    {
        if (isOver) return;
        float moveInput = 0f;
        bool moveH = false;

        if (Input.GetKey(KeyCode.A))
        {
            moveInput = -1f;
            spriteRenderer.flipX = true;
            moveH = true;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveInput = 1f;
            spriteRenderer.flipX = false;
            moveH = true;
        }

        float vx = moveInput * moveSpeed;
        float vy = rb.velocityY;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer) && rb.velocityY == 0;
        if (isFalling && isGrounded) //처음으로 땅을 만났을 때
        {
            jumpCount = maxJumpCount;
            isFalling = false;
            animator.SetBool("IsFalling", false);
        }

        if (Input.GetKeyDown(KeyCode.Space) && jumpCount > 0)
        {
            jumpCount--;
            vy = jumpForce;
            if (isGrounded)
            {
                animator.SetTrigger("IsJump");
            }
            else
            {
                isFalling = false;
                animator.SetBool("IsFalling", false);
                animator.SetTrigger("IsDoubleJump");
            }
        }

        if (!isFalling && !isGrounded && vy < 0) { //처음으로 떨어지기 시작할 때
            isFalling = true;
            animator.SetBool("IsRun", false);
            animator.SetBool("IsIDLE", false);
            animator.SetBool("IsFalling", true);
        }

        rb.velocity = new Vector2(vx, vy);

        if (!isGrounded) return;

        if (moveH)
        {
            animator.SetBool("IsRun", true);
            animator.SetBool("IsIDLE", false);
        }
        else
        {
            animator.SetBool("IsRun", false);
            animator.SetBool("IsIDLE", true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            HP--;
            var obj = Instantiate(hitPrefab, transform.position, Quaternion.identity);
            Destroy(obj, 0.5f);
            animator.SetTrigger("IsHit");
            HPText.text = $"HP: {HP}";
            slider.value = (float)HP / maxHP;

            if (HP <= 0)
            {
                gameOver.SetActive(true);
                isOver = true;
                Destroy(gameObject);
            }
        }
        else if (collision.gameObject.CompareTag("Tram"))
        {
            rb.velocityY = jumpForce * 1.5f;
            var tramAnimator = collision.gameObject.GetComponent<Animator>();
            tramAnimator.SetTrigger("IsJump");
            isFalling = false;
            animator.SetBool("IsFalling", false);
            animator.SetTrigger("IsJump");
            jumpCount = maxJumpCount - 1;
        }
        else if (collision.gameObject.CompareTag("End"))
        {
            gameClear.SetActive(true);
            isOver = true;
            rb.velocity = Vector2.zero;
        }
    }
}
