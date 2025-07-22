using System.Collections;
using UnityEngine;

public abstract class LauncherBase : MonoBehaviour
{
    [Header("Launch Settings")]
    [SerializeField] protected Transform initialLaunchPoint;
    [SerializeField] protected float launchForceMultiplier = 10f;
    [SerializeField] protected float maxLaunchForce = 10f;

    protected Transform currentLaunchPoint;

    // 조준(각도) 및 파워 조절 상태 플래그
    protected bool isPowerHandling = false;
    protected bool isAngleHandling = false;

    // ⭐️ 스턴 상태 플래그/남은시간
    protected bool isStunned = false;
    protected float stunTimer = 0f;

    protected virtual void Start()
    {
        currentLaunchPoint = initialLaunchPoint;
    }

    // --------------------------
    //      스턴 관리 부분
    // --------------------------

    /// <summary>
    /// (Meteor 등에서 호출) 스턴 + 강제 발사
    /// </summary>
    public virtual void StunAndForceShoot(Vector2 direction, float power, float stunDuration = 1.5f)
    {
        UnityEngine.Debug.Log($"[LauncherBase] StunAndForceShoot 호출! 방향:{direction}, 파워:{power}, 스턴:{stunDuration}초");
        StartStun(stunDuration);
        LaunchBallByVector(direction, power);
    }

    /// <summary>
    /// 스턴 시작 (플래그 ON, 타이머)
    /// </summary>
    public virtual void StartStun(float duration)
    {
        if (isStunned) return; // 이미 스턴 상태면 중복 방지
        isStunned = true;
        stunTimer = duration;
        UnityEngine.Debug.Log($"[LauncherBase] {duration}초 동안 스턴");
        StartCoroutine(StunTimerCoroutine(duration));
    }

    private IEnumerator StunTimerCoroutine(float duration)
    {
        float timer = duration;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        EndStun();
    }

    /// <summary>
    /// 스턴 해제
    /// </summary>
    public virtual void EndStun()
    {
        isStunned = false;
        stunTimer = 0f;
        UnityEngine.Debug.Log("[LauncherBase] 스턴 해제");
    }

    /// <summary>
    /// 현재 스턴 상태 체크용 (필요시 public getter)
    /// </summary>
    public bool IsStunned() => isStunned;

    // --------------------------
    //    기본 런처 기능 부분
    // --------------------------

    public abstract void LaunchBall(float angle);

    public virtual void UpdateLaunchPoint(Transform newLaunchPoint)
    {
        currentLaunchPoint = newLaunchPoint;
        UnityEngine.Debug.Log("Launch point updated: " + currentLaunchPoint.position);
    }

    // 파워 조절 상태
    public void SetPowerHandlingState(bool state) { isPowerHandling = state; }
    public bool GetPowerHandlingState() { return isPowerHandling; }

    // 각도 조절 상태
    public void SetAngleHandlingState(bool state) { isAngleHandling = state; }
    public bool GetAngleHandlingState() { return isAngleHandling; }

    /// <summary>
    /// Meteor 충돌 등에서 "현재 조준/충전 중" 체크
    /// </summary>
    public bool IsAimingOrPowerHandling()
    {
        return isAngleHandling || isPowerHandling;
    }

    // ⭐️ 현재 파워 조절만 하고 있는지 (for Meteor)
    public bool IsPowerHandlingOnly()
    {
        return isPowerHandling && !isAngleHandling;
    }

    // ⭐️ 현재 각도 조절만 하고 있는지 (for Meteor)
    public bool IsAngleHandlingOnly()
    {
        return !isPowerHandling && isAngleHandling;
    }

    /// <summary>
    /// ⭐️ 방향 벡터 + 파워로 발사 (파생 클래스에서 구현)
    /// </summary>
    public virtual void LaunchBallByVector(Vector2 direction, float power)
    {
        UnityEngine.Debug.Log($"[Meteor 강제발사] 방향: {direction}, 파워: {power}");
        // DefaultLauncher에서 override 하여 실제 발사 로직 구현
    }
}
