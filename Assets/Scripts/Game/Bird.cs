using UnityEngine;

public class Bird : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isFalling = false;
    private bool isChasing = false;
    private Transform targetBall;
    [SerializeField] private GameObject Redeye;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }
    void Start()
    {
        // ������(x=1)���� �ӵ� 5 �ֱ�
        rb.velocity = new Vector2(5f, 0f);  // ����: rb.velocity = new Vector2(-5f, 0f);
    }
    void Update()
    {
        // 추적 상태면 타겟을 향해 돌진
        if (isChasing && targetBall != null && !isFalling)
        {
            Vector2 dir = (targetBall.position - transform.position).normalized;
            float chaseSpeed = 8f; // 추적 속도 조절
            rb.velocity = dir * chaseSpeed;
        }
        // 낙하 상태에서 y가 충분히 낮아지면 파괴
        if (isFalling && transform.position.y < -10f)
        {
            Destroy(gameObject);
        }
        // Redeye 활성화/비활성화
        if (Redeye != null)
            Redeye.SetActive(isChasing);
    }

    // 감지용 트리거 콜라이더에서 처리
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isChasing && !isFalling && other.CompareTag("Ball"))
        {
            isChasing = true;
            targetBall = other.transform;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (isChasing && other.transform == targetBall)
        {
            isChasing = false;
            targetBall = null;
        }
    }

    // 충돌용 박스 콜라이더에서 처리
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isFalling && collision.gameObject.CompareTag("Ball"))
        {
            // 추적 종료, 낙하 시작
            isChasing = false;
            isFalling = true;
            targetBall = null;
            rb.velocity = Vector2.down * 5f;
            rb.gravityScale = 2f;
            UnityEngine.Debug.Log("Bird가 Ball과 충돌 후 아래로 낙하!");
            Collider2D myCol = GetComponent<Collider2D>();
            if (myCol != null)
                myCol.enabled = false;
            if (Redeye != null)
                Redeye.SetActive(false);
        }
    }
}
