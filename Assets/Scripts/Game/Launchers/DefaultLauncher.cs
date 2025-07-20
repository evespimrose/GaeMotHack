using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AimInputHandler))]
public class DefaultLauncher : LauncherBase, IAimInputHandler
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private float maxPreviewDistance = 3f; // 조준 중 최대 미리보기 거리
    [SerializeField] private Scrollbar powerGauge;
    [SerializeField] private float currentPower;

    private Vector2 startPosition;
    private GameObject previewBallInstance;

    private AimInputHandler inputHandler;

    protected override void Start() // override 키워드 추가
    {
        inputHandler = GetComponent<AimInputHandler>();
        if (inputHandler == null)
            UnityEngine.Debug.LogError("AimInputHandler가 필요합니다."); // UnityEngine.Debug 명시

        currentLaunchPoint = initialLaunchPoint;
    }

    // IAimInputHandler 인터페이스 구현
    private void Update()
    {
        // currentPower가 바뀔 때마다 Scrollbar 값 반영 (0 ~ 1 사이)
        if (powerGauge != null)
        {
            powerGauge.value = Mathf.Clamp01(currentPower);
        }
    }


    public void OnStartAiming(Vector2 position)
    {
        // 각도 조절 시작 시 호출
        SetAngleHandlingState(true);
        UnityEngine.Debug.Log("각도 조절 시작");
    }

    public void OnEndAiming(Vector2 position)
    {
        // 각도 조절 종료 시 호출
        SetAngleHandlingState(false);
        float angle = position.x; // AimInputHandler에서 angle 값을 넘겨주도록 수정
        UnityEngine.Debug.Log("각도 조절 종료, 각도: " + angle);
    }

    public void OnStartPowerHandling()
    {
        // 파워 조절 시작 시 호출
        SetPowerHandlingState(true);
        UnityEngine.Debug.Log("파워 조절 시작");
    }

    public void OnEndPowerHandling(float power)
    {
        // 파워 조절 종료 시 호출
        SetPowerHandlingState(false);
        currentPower = power;

        UnityEngine.Debug.Log("파워 조절 종료, 파워: " + currentPower);
        //모든 조절이 끝났으면 발사
        if (!GetAngleHandlingState())
        {
            LaunchBall();
        }
    }

    // LauncherBase 추상 메서드 구현
    public override void LaunchBall(Vector2 direction, float normalizedPower)
    {
        // 사용하지 않음
    }

    // 모든 조절이 끝났을 때 호출되는 발사 메서드
    private void LaunchBall()
    {
        //AimInputHandler에서 Angle 값 가져오기
        float angle = inputHandler.GetAngle();

        // 정규화된 파워를 사용
        GameObject instance = Instantiate(ballPrefab, currentLaunchPoint.position, Quaternion.identity);

        Ball ball = instance.GetComponent<Ball>();
        if (ball == null)
        {
            UnityEngine.Debug.LogError("Ball prefab에 Ball 컴포넌트가 없습니다.");
            return;
        }

        // 각도를 0~180도로 변환
        float launchAngle = angle * 180f;

        // 발사 방향 계산 (2D)
        Vector2 launchDirection = new Vector2(Mathf.Cos(launchAngle * Mathf.Deg2Rad), Mathf.Sin(launchAngle * Mathf.Deg2Rad));

        // Ball 컴포넌트의 Launch 메서드 호출
        ball.Launch(launchDirection, currentPower * launchForceMultiplier);

        UnityEngine.Debug.Log("발사! 방향: " + launchDirection + ", 파워: " + currentPower);
    }
}
