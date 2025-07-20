using System.Diagnostics;
using UnityEngine;

public abstract class LauncherBase : MonoBehaviour
{
    [Header("Launch Settings")]
    [SerializeField] protected Transform initialLaunchPoint;
    [SerializeField] protected float launchForceMultiplier = 10f;
    [SerializeField] protected float maxLaunchForce = 10f;
    protected float currentLaunchForce = 0f;

    protected Transform currentLaunchPoint;

    protected bool isPowerHandling = false; // 파워 조절 상태
    protected bool isAngleHandling = false; // 각도 조절 상태

    protected virtual void Start()
    {
        currentLaunchPoint = initialLaunchPoint;
    }

    /// <summary>
    /// 런처에서 실제로 발사하는 기능. 상속한 클래스에서 구현
    /// </summary>
    public abstract void LaunchBall(Vector2 direction, float normalizedPower);

    /// <summary>
    /// 런치 포인트 위치 갱신
    /// </summary>
    public virtual void UpdateLaunchPoint(Transform newLaunchPoint)
    {
        currentLaunchPoint = newLaunchPoint;
        UnityEngine.Debug.Log("Launch point updated: " + currentLaunchPoint.position);
    }

    // 파워 조절 상태 설정
    public void SetPowerHandlingState(bool state)
    {
        isPowerHandling = state;
    }

    // 각도 조절 상태 설정
    public void SetAngleHandlingState(bool state)
    {
        isAngleHandling = state;
    }

    public bool GetPowerHandlingState()
    {
        return isPowerHandling;
    }

    public bool GetAngleHandlingState()
    {
        return isAngleHandling;
    }
}
