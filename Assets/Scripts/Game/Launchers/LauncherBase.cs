using System.Diagnostics;
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

    protected virtual void Start()
    {
        currentLaunchPoint = initialLaunchPoint;
    }

    // 상태 쿼리 함수
    public bool IsPowerHandlingOnly() => isPowerHandling && !isAngleHandling;
    public bool IsAngleHandlingOnly() => isAngleHandling && !isPowerHandling;
    public bool IsIdleState() => !isAngleHandling && !isPowerHandling;

    public void SetPowerHandlingState(bool state) { isPowerHandling = state; }
    public bool GetPowerHandlingState() { return isPowerHandling; }

    public void SetAngleHandlingState(bool state) { isAngleHandling = state; }
    public bool GetAngleHandlingState() { return isAngleHandling; }

    // Meteor 충돌 시 기절 및 강제발사 처리
    public virtual void StunAndForceShoot(Vector2 direction, float minPower)
    {
        LaunchBallByVector(direction, minPower);
    }
    public virtual void LaunchBallByVector(Vector2 direction, float power)
    {
        UnityEngine.Debug.Log($"[Meteor 강제발사] 방향: {direction}, 파워: {power}");
        // 실제 발사는 DefaultLauncher에서 구현
    }

    public abstract void LaunchBall(float angle);

    public virtual void UpdateLaunchPoint(Transform newLaunchPoint)
    {
        currentLaunchPoint = newLaunchPoint;
        UnityEngine.Debug.Log("Launch point updated: " + currentLaunchPoint.position);
    }
}
