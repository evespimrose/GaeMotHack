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

    protected override void Start()
    {
        base.Start();
        inputHandler = GetComponent<AimInputHandler>();
        if (inputHandler == null)
            Debug.LogError("AimInputHandler가 필요합니다.");
    }
    // IAimInputHandler 인터페이스 구현
    private void Update()
    {
        if (GetPowerHandlingState() && inputHandler != null)
        {
            currentPower = inputHandler.GetPower();
        }

        // currentPower가 바뀔 때마다 Scrollbar 값 반영 (0 ~ 1 사이)
        if (powerGauge != null)
        {
            powerGauge.value = Mathf.Clamp01(currentPower);
        }

        if (aimPanel != null)
        {
            aimPanel.transform.rotation = Quaternion.Euler(0f, 0f, currentAimAngle);
        }

    }

    public void Aim(bool active)
    {
        aimPanel.gameObject.SetActive(active);
    }

    // IAimInputHandler 구현부
    public void OnStartAiming(Vector2 position)
    {
        SetAngleHandlingState(true);
        Debug.Log("각도 조절 시작");
        isReadyToShoot = false;
    }

    public void OnEndAiming(Vector2 position)
    {
        SetAngleHandlingState(false);
        float angle = inputHandler.GetAngle();
        Debug.Log("각도 조절 종료, 각도: " + angle);
        // 파워 조절 시작은 AimInputHandler에서 handler.OnStartPowerHandling()로 자동 호출됨
    }

    public void OnStartPowerHandling()
    {
        SetPowerHandlingState(true);
        Debug.Log("파워 조절 시작");
        isReadyToShoot = false;
    }

    public void OnEndPowerHandling(float power)
    {
        SetPowerHandlingState(false);
        currentPower = power;
        Debug.Log("파워 조절 종료, 파워: " + currentPower);
        LaunchBall(inputHandler.GetAngle());
        Destroy(gameObject); // ⭐️ 공 발사 후 발사대(Launcher) 삭제 ⭐️
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
        Debug.Log("발사! 방향: " + launchDirection + ", 파워: " + currentPower);
    }
}
