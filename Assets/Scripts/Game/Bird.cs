using UnityEngine;

public class Bird : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isFalling = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }
    void Start()
    {
        // 오른쪽(x=1)으로 속도 5 주기
        rb.velocity = new Vector2(5f, 0f);  // 왼쪽: rb.velocity = new Vector2(-5f, 0f);
    }
    void Update()
    {
        // 아래로 낙하 중 && 충분히 아래로 떨어지면 삭제
        if (isFalling && transform.position.y < -10f)
        {
            Destroy(gameObject);
        }
    }
    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isFalling && collision.gameObject.CompareTag("Boal"))
        {
            // 충돌 시 수직 낙하
            isFalling = true;
            rb.velocity = Vector2.down * 5f;
            rb.gravityScale = 2f;
            UnityEngine.Debug.Log("Bird가 Ball과 충돌 → 아래로 낙하!");

            Collider2D myCol = GetComponent<Collider2D>();
            if (myCol != null)
                myCol.enabled = false;
        }
    }
}
