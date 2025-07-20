using UnityEngine;

[RequireComponent(typeof(AimInputHandler))]
public class DefaultLauncher : LauncherBase, IAimInputHandler
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private float maxPreviewDistance = 3f; // 조준 중 최대 미리보기 거리

    private Vector2 startPosition;
    private GameObject previewBallInstance;

    private AimInputHandler inputHandler;

    private void Awake()
    {
        inputHandler = GetComponent<AimInputHandler>();
        if (inputHandler == null)
            Debug.LogError("AimInputHandler가 필요합니다.");
    }

    private void Start()
    {
        base.Start();
    }

    public void OnStartAiming(Vector2 position)
    {
        startPosition = position;

        // 조준용 공 생성
        if (previewBallInstance == null)
        {
            previewBallInstance = Instantiate(ballPrefab, currentLaunchPoint.position, Quaternion.identity);
            Rigidbody2D rb = previewBallInstance.GetComponent<Rigidbody2D>();
            if (rb != null) rb.isKinematic = true;
        }
    }

    public void OnUpdateAiming(Vector2 currentPosition)
    {
        if (previewBallInstance == null) return;

        Vector2 aimVector = startPosition - currentPosition;
        float clampedMagnitude = Mathf.Min(aimVector.magnitude, maxPreviewDistance);

        Vector2 clampedVector = aimVector.normalized * clampedMagnitude;
        Vector2 ballPosition = startPosition - clampedVector;

        previewBallInstance.transform.position = ballPosition;
    }

    public void OnEndAiming(Vector2 position)
    {
        Vector2 launchVector = startPosition - position;
        float forceMagnitude = Mathf.Clamp(launchVector.magnitude, 0, maxLaunchForce);
        float normalizedForce = forceMagnitude / maxLaunchForce;
        Vector2 direction = launchVector.normalized;

        // 발사
        LaunchBall(direction, normalizedForce);

        // 미리보기 공 제거
        if (previewBallInstance != null)
        {
            Destroy(previewBallInstance);
            previewBallInstance = null;
        }
    }

    public override void LaunchBall(Vector2 direction, float normalizedPower)
    {
        GameObject instance = Instantiate(ballPrefab, currentLaunchPoint.position, Quaternion.identity);

        Ball ball = instance.GetComponent<Ball>();
        if (ball == null)
        {
            Debug.LogError("Ball prefab에 Ball 컴포넌트가 없습니다.");
            return;
        }

        ball.Launch(direction, normalizedPower * launchForceMultiplier);
    }
}
