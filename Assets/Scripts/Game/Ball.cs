using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] public float force;
    [SerializeField] private float power;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Ball 오브젝트에 Rigidbody2D 컴포넌트가 없습니다.");
        }
    }

    /// <summary>
    /// 공을 주어진 방향과 힘으로 발사합니다.
    /// </summary>
    public void Launch(Vector2 direction, float power)
    {
        rb.velocity = Vector2.zero; // 기존 속도 초기화
        this.power = power;
        rb.AddForce(direction.normalized * power, ForceMode2D.Impulse);
        Debug.Log($"[Ball.Launch] 호출됨: 방향 {direction}, 파워 {power}");
        Debug.Log($"[Ball.Launch] AddForce 적용 후 velocity: {rb.velocity}");
    }

    /// <summary>
    /// 발사대가 제공하는 효과를 적용합니다.
    /// </summary>
    public void ApplyEffect(ILauncherEffect effect)
    {
        effect?.ApplyEffect(rb);
    }

    private void Update()
    {
        force = rb.velocity.magnitude;
    }

    /// <summary>
    /// 골에 도달했는지 체크하는 예시 Trigger
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Goal"))
        {
            Debug.Log("🎉 Goal Reached!");
            GameManager.Instance.StageComplete();
        }
        
        if (other.CompareTag("TrapGoal"))
        {
            Debug.Log("TrapGoal에 닿음!");
            if (rb.velocity.sqrMagnitude > 0.01f)
            {
                Vector2 bounceDir = -rb.velocity.normalized;
                float bouncePower = 5f; // 강한 힘
                rb.velocity = Vector2.zero; // 기존 속도 초기화
                rb.AddForce(bounceDir * bouncePower, ForceMode2D.Impulse);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            // Fairway일 시 마찰력 계수 (예: 0.98f, 1에 가까울수록 천천히 감속)
            float friction = 0.98f;
            rb.velocity *= friction;
        }
    }
}
