using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeHead : MonoBehaviour
{
    public Transform leftEnd;
    public Transform rightEnd;
    public Rigidbody2D rb;
    public float smoothTime = 0.3f; //�ε巯�� ����
    public float maxSpeed = 5f; // �ִ� �ӵ�
    private Vector2 currentVelocity;
    private Vector2 targetPos;
    private bool goRight = true;

    void Start()
    {
        targetPos = rightEnd.position;
    }

    void FixedUpdate()
    {
        // ���� ��ġ�� ��ǥ ��ġ ���̸� �ε巴�� ����
        Vector2 newPos = Vector2.SmoothDamp(
            rb.position, 
            targetPos,
            ref currentVelocity, 
            smoothTime, 
            maxSpeed, 
            Time.fixedDeltaTime
        );
        rb.MovePosition(newPos);

        // ��ǥ�� ��������� ���� ����
        if (Vector2.Distance(rb.position, targetPos) < 0.05f)
        {
            goRight = !goRight;
            targetPos = goRight ? rightEnd.position : leftEnd.position;
        }
    }
}
