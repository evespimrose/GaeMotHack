using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AimInputHandler))]
public class DefaultLauncher : LauncherBase, IAimInputHandler
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Scrollbar powerGauge;
    [SerializeField] private GameObject aimPanel;
    [SerializeField] private float currentPower;
    [SerializeField] private float currentAimAngle;

    private AimInputHandler inputHandler;
    private bool isReadyToShoot = false;

    // ⭐️ 현재 파워/각도 외부 제공용 프로퍼티
    public float CurrentPower => currentPower;
    public float CurrentAngle => inputHandler != null ? inputHandler.GetAngle() : 0f;

    protected override void Start()
    {
        base.Start();
        inputHandler = GetComponent<AimInputHandler>();
        if (inputHandler == null)
            UnityEngine.Debug.LogError("AimInputHandler가 필요합니다.");
    }

    private void Update()
    {
        if (GetPowerHandlingState() && inputHandler != null)
            currentPower = inputHandler.GetPower();

        if (powerGauge != null)
            powerGauge.value = Mathf.Clamp01(currentPower);

        if (GetAngleHandlingState() && inputHandler != null)
            currentAimAngle = inputHandler.GetAngle();

        if (aimPanel != null)
            aimPanel.transform.rotation = Quaternion.Euler(0f, 0f, currentAimAngle);
    }

    public void Aim(bool active)
    {
        aimPanel.gameObject.SetActive(active);
    }

    // IAimInputHandler 구현부
    public void OnStartAiming(Vector2 position)
    {
        SetAngleHandlingState(true);
        UnityEngine.Debug.Log("각도 조절 시작");
        isReadyToShoot = false;
    }

    public void OnEndAiming(Vector2 position)
    {
        SetAngleHandlingState(false);
        float angle = inputHandler.GetAngle();
        UnityEngine.Debug.Log("각도 조절 종료, 각도: " + angle);
        // 파워 조절 시작은 AimInputHandler에서 handler.OnStartPowerHandling()로 자동 호출됨
    }

    public void OnStartPowerHandling()
    {
        SetPowerHandlingState(true);
        UnityEngine.Debug.Log("파워 조절 시작");
        isReadyToShoot = false;
    }

    public void OnEndPowerHandling(float power)
    {
        SetPowerHandlingState(false);
        currentPower = power;
        UnityEngine.Debug.Log("파워 조절 종료, 파워: " + currentPower);
        LaunchBall(inputHandler.GetAngle());
        Destroy(gameObject); // 공 발사 후 발사대(Launcher) 삭제
    }

    public override void LaunchBall(float angle)
    {
        GameObject instance = Instantiate(ballPrefab, currentLaunchPoint.position, Quaternion.identity);
        Ball ball = instance.GetComponent<Ball>();
        if (ball == null)
        {
            UnityEngine.Debug.LogError("Ball prefab에 Ball 컴포넌트가 없습니다.");
            return;
        }
        float radianAngle = angle * Mathf.Deg2Rad;
        Vector2 launchDirection = new Vector2(Mathf.Cos(radianAngle), Mathf.Sin(radianAngle));
        ball.Launch(launchDirection, currentPower * launchForceMultiplier);
        UnityEngine.Debug.Log("발사! 방향: " + launchDirection + ", 파워: " + currentPower);
    }

    // ⭐️ Meteor 충돌 등에서 방향/파워 직접 지정 강제발사
    public override void LaunchBallByVector(Vector2 direction, float power)
    {
        if (ballPrefab == null || currentLaunchPoint == null)
        {
            UnityEngine.Debug.LogError("Ball 프리팹 또는 런치 포인트가 연결되지 않았습니다.");
            return;
        }
        GameObject instance = Instantiate(ballPrefab, currentLaunchPoint.position, Quaternion.identity);
        Ball ball = instance.GetComponent<Ball>();
        if (ball == null)
        {
            UnityEngine.Debug.LogError("Ball prefab에 Ball 컴포넌트가 없습니다.");
            return;
        }
        ball.Launch(direction.normalized, power * launchForceMultiplier);
        UnityEngine.Debug.Log($"[Meteor 강제발사] 방향: {direction.normalized}, 파워: {power}");
        Destroy(gameObject); // 반드시 마지막에!
    }

}
