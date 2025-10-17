using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public GameObject hitPrefab;
    public UIManager uiManager;

    public int maxHP;
    public int maxJumpCount = 2;
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float gameOverHeight;

    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded;
    private bool isOver;
    private int jumpCount;
    private int HP;
    private bool isFalling;
    private Animator animator;
    private int itemCount = 0;
    private int hitCount = 0;

    private List<string> animeParameters = new()
    {
        "IsIDLE", "IsRun", "IsJump", "IsFalling"
    };

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
        uiManager.init(HP);
    }

    void Update()
    {
        if (isOver) return;

        if (transform.position.y < gameOverHeight)
        {
            PlayHitEffect();
            isOver = true;
            gameObject.SetActive(false);
            uiManager.GameOver();
        }

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

        bool groundTest = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer) && rb.velocityY <= 1e-4;
        if (!isGrounded && groundTest) //처음으로 땅을 만났을 때
        {
            isGrounded = true;
            jumpCount = maxJumpCount;
            isFalling = false;
            animator.SetBool("IsFalling", false);
            animator.SetBool("IsJump", false);
        }
        if (!groundTest)
        {
            isGrounded = false;
        }

        bool hasJumped = false;
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount > 0)
        {
            hasJumped = true;
            jumpCount--;
            vy = jumpForce;
            SetParameterOnlyTrue("IsJump");
            if (!isGrounded)
            {
                isFalling = false;
                animator.SetTrigger("IsDoubleJump");
            }
        }

        if (!isFalling && !isGrounded && vy < 0) { //처음으로 떨어지기 시작할 때
            isFalling = true;
            SetParameterOnlyTrue("IsFalling");
        }

        rb.velocity = new Vector2(vx, vy);

        if (!isGrounded || hasJumped) return;

        if (moveH)
        {
            SetParameterOnlyTrue("IsRun");
        }
        else
        {
            SetParameterOnlyTrue("IsIDLE");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            HP--;
            hitCount++;
            PlayHitEffect();
            animator.SetTrigger("IsHit");
            uiManager.SetHP(HP);

            if (HP <= 0)
            {
                isOver = true;
                gameObject.SetActive(false);
                uiManager.GameOver();
            }
        }
        else if (collision.gameObject.CompareTag("Tram"))
        {
            rb.velocityY = jumpForce * 1.5f;
            var tramAnimator = collision.gameObject.GetComponent<Animator>();
            tramAnimator.SetTrigger("IsJump");
            isFalling = false;
            SetParameterOnlyTrue("IsJump");
            jumpCount = maxJumpCount - 1;
        }
        else if (collision.gameObject.CompareTag("Item"))
        {
            int hpPlus = 1;
            itemCount++;
            if (collision.name == "Banana")
            {
                hpPlus = 2;
            }
            if (HP < maxHP)
            {
                HP = Mathf.Min(maxHP, HP + hpPlus);
                uiManager.SetHP(HP);
            }
            var itemAnimator = collision.gameObject.GetComponent<Animator>();
            itemAnimator.SetTrigger("Collected");
            Destroy(collision.gameObject, 0.5f);
        }
        else if (collision.gameObject.CompareTag("End"))
        {
            uiManager.GameClear(itemCount, hitCount);
            isOver = true;
            rb.velocity = Vector2.zero;
        }
    }

    private void PlayHitEffect()
    {
        var obj = Instantiate(hitPrefab, transform.position, Quaternion.identity);
        Destroy(obj, 0.5f);
    }

    private void SetParameterOnlyTrue(string param)
    {
        foreach (var p in animeParameters)
        {
            if (p == param) animator.SetBool(p, true);
            else animator.SetBool(p, false);
        }
    }
}
