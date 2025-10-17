using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeHead : MonoBehaviour
{
    public Transform leftEnd;
    public Transform rightEnd;
    public Rigidbody2D rb;
    public float smoothTime = 0.3f; //부드러움 정도
    public float maxSpeed = 5f; // 최대 속도
    private Vector2 currentVelocity;
    private Vector2 targetPos;
    private bool goRight = true;

    void Start()
    {
        targetPos = rightEnd.position;
    }

    void FixedUpdate()
    {
        // 현재 위치와 목표 위치 사이를 부드럽게 보간
        Vector2 newPos = Vector2.SmoothDamp(
            rb.position, 
            targetPos,
            ref currentVelocity, 
            smoothTime, 
            maxSpeed, 
            Time.fixedDeltaTime
        );
        rb.MovePosition(newPos);

        // 목표에 가까워지면 방향 반전
        if (Vector2.Distance(rb.position, targetPos) < 0.05f)
        {
            goRight = !goRight;
            targetPos = goRight ? rightEnd.position : leftEnd.position;
        }
    }
}
