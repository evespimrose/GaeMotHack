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

    [SerializeField] private Canvas powerCanvas;

    // 현재 파워/각도 외부 제공용 프로퍼티
    public float CurrentPower => currentPower;
    public float CurrentAngle => inputHandler != null ? inputHandler.GetAngle() : 0f;

    protected override void Start()
    {
        base.Start();
        inputHandler = GetComponent<AimInputHandler>();
        if (inputHandler == null)
            Debug.LogError("AimInputHandler가 필요합니다.");
    }

    private void Update()
    {
        // ⭐️ 기절 상태일 땐 입력/업데이트 등 전부 동작 정지
        if (IsStunned()) return;

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

    // IAimInputHandler 구현부는 그대로
    public void OnStartAiming(Vector2 position)
    {
        if (IsStunned()) return;
        SetAngleHandlingState(true);
        Debug.Log("각도 조절 시작");
        isReadyToShoot = false;
    }

    public void OnEndAiming(Vector2 position)
    {
        if (IsStunned()) return;
        SetAngleHandlingState(false);
        float angle = inputHandler.GetAngle();
        Debug.Log("각도 조절 종료, 각도: " + angle);
    }

    public void OnStartPowerHandling()
    {
        if (IsStunned()) return;
        SetPowerHandlingState(true);
        Debug.Log("파워 조절 시작");
        isReadyToShoot = false;
    }

    public void OnEndPowerHandling(float power)
    {
        if (IsStunned()) return;
        SetPowerHandlingState(false);
        currentPower = power;
        Debug.Log("파워 조절 종료, 파워: " + currentPower);
        LaunchBall(inputHandler.GetAngle());
        Destroy(gameObject); // 공 발사 후 발사대(Launcher) 삭제
    }

    public override void LaunchBall(float angle)
    {
        GameObject instance = Instantiate(ballPrefab, currentLaunchPoint.position, Quaternion.identity);
        Ball ball = instance.GetComponent<Ball>();
        if (ball == null)
        {
            Debug.LogError("Ball prefab에 Ball 컴포넌트가 없습니다.");
            return;
        }
        float radianAngle = angle * Mathf.Deg2Rad;
        Vector2 launchDirection = new Vector2(Mathf.Cos(radianAngle), Mathf.Sin(radianAngle));
        ball.Launch(launchDirection, currentPower * launchForceMultiplier);
        GameManager.Instance?.RegisterBall(instance);
        Debug.Log("발사! 방향: " + launchDirection + ", 파워: " + currentPower + "배수 : " + launchForceMultiplier);
    }

    public override void LaunchBallByVector(Vector2 direction, float power)
    {
        if (ballPrefab == null || currentLaunchPoint == null)
        {
            Debug.LogError("Ball 프리팹 또는 런치 포인트가 연결되지 않았습니다.");
            return;
        }
        GameObject instance = Instantiate(ballPrefab, currentLaunchPoint.position, Quaternion.identity);
        Ball ball = instance.GetComponent<Ball>();
        if (ball == null)
        {
            Debug.LogError("Ball prefab에 Ball 컴포넌트가 없습니다.");
            return;
        }
        Debug.Log($"[Meteor 강제발사] 방향: {direction.normalized}, 파워: {power}, 배수 : {launchForceMultiplier} = {power * launchForceMultiplier}");
        ball.Launch(direction.normalized, power * launchForceMultiplier);
        GameManager.Instance?.RegisterBall(instance);
        Destroy(gameObject);
    }

    // powerCanvas의 RenderMode가 WorldSpace일 때 카메라를 설정하는 메서드
    public void SetPowerCanvasCamera(Camera cam)
    {
        if (powerCanvas == null)
            powerCanvas = GetComponentInChildren<Canvas>();
        if (powerCanvas != null && powerCanvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            powerCanvas.worldCamera = cam;
            Debug.Log($"powerCanvas.worldCamera : {powerCanvas.worldCamera.name}, cam : {cam.name}");
        }
    }
}
