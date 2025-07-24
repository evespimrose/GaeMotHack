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
        // ������(x=1)���� �ӵ� 5 �ֱ�
        rb.velocity = new Vector2(5f, 0f);  // ����: rb.velocity = new Vector2(-5f, 0f);
    }
    void Update()
    {
        // �Ʒ��� ���� �� && ����� �Ʒ��� �������� ����
        if (isFalling && transform.position.y < -10f)
        {
            Destroy(gameObject);
        }
    }
    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isFalling && collision.gameObject.CompareTag("Boal"))
        {
            // �浹 �� ���� ����
            isFalling = true;
            rb.velocity = Vector2.down * 5f;
            rb.gravityScale = 2f;
            UnityEngine.Debug.Log("Bird�� Ball�� �浹 �� �Ʒ��� ����!");

            Collider2D myCol = GetComponent<Collider2D>();
            if (myCol != null)
                myCol.enabled = false;
        }
    }
}
