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
    public Animator TramAnimator;
    public int MaxHP;

    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded;
    private bool isOver;
    private int HP;

    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        isOver = false;
        HP = MaxHP;
        HPText.text = $"HP: {MaxHP}";
    }

    void Update()
    {
        if (isOver) return;
        float moveInput = 0f;

        if (Input.GetKey(KeyCode.A))
        {
            moveInput = -1f;
            spriteRenderer.flipX = true;
            AnimatorChange("IsRun");
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveInput = 1f;
            spriteRenderer.flipX = false;
            AnimatorChange("IsRun");
        }
        else
        {
            AnimatorChange("IsIDLE");
        }

        float vx = moveInput * moveSpeed;
        float vy = rb.velocityY;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            vy = jumpForce;
            AnimatorChange("IsJump");
        }

        rb.velocity = new Vector2(vx, vy);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            HP--;
            var obj = Instantiate(hitPrefab, transform.position, Quaternion.identity);
            Destroy(obj, 0.5f);
            animator.SetTrigger("IsHit");
            HPText.text = $"HP: {HP}";
            slider.value = (float)HP / MaxHP;

            if (HP <= 0)
            {
                gameOver.SetActive(true);
                isOver = true;
                Destroy(gameObject);
            }
        }
        else if (collision.gameObject.tag == "Tram")
        {
            rb.velocityY = jumpForce * 1.5f;
            TramAnimator.SetTrigger("IsJump");
            AnimatorChange("IsJump");
        }
        else if (collision.gameObject.tag == "End")
        {
            gameClear.SetActive(true);
            isOver = true;
            rb.velocity = Vector2.zero;
        }
    }

    private void AnimatorChange(string name)
    {
        animator.SetBool("IsIDLE", false);
        animator.SetBool("IsRun", false);

        if (name == "IsJump")
        {
            animator.SetTrigger("IsJump");
            return;
        }
        animator.SetBool(name, true);
    }
}
