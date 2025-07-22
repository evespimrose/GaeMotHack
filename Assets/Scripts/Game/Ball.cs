using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            UnityEngine.Debug.LogError("Ball 오브젝트에 Rigidbody2D 컴포넌트가 없습니다.");
        }
    }

    /// <summary>
    /// 공을 주어진 방향과 힘으로 발사합니다.
    /// </summary>
    public void Launch(Vector2 direction, float power)
    {
        rb.velocity = Vector2.zero; // 기존 속도 초기화
        rb.AddForce(direction.normalized * power, ForceMode2D.Impulse);
        UnityEngine.Debug.Log($"[Ball.Launch] 호출됨: 방향 {direction}, 파워 {power}");
        UnityEngine.Debug.Log($"[Ball.Launch] AddForce 적용 후 velocity: {rb.velocity}");
    }

    /// <summary>
    /// 발사대가 제공하는 효과를 적용합니다.
    /// </summary>
    public void ApplyEffect(ILauncherEffect effect)
    {
        effect?.ApplyEffect(rb);
    }

    /// <summary>
    /// 골에 도달했는지 체크하는 예시 Trigger
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Goal"))
        {
            UnityEngine.Debug.Log("🎉 Goal Reached!");
            // GameManager.Instance.LevelComplete(); 등과 연동
        }
    }
}
