using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int topLevel = 0;

    public int TopLevel() => topLevel;

    // 골 도달 이벤트
    public event Action GameCleared;

    // 메테오 이벤트 (위치 포함)
    public event Action<Vector3> MeteorOccurred;

    [SerializeField] public GameObject CurrentBall { get; private set; }

    [Header("런처 프리팹")]
    public GameObject launcherPrefab;

    private ClearUI clearUI;

    public Camera GMHCamera;

    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 필요 시 로딩 시 처리할 로직 작성
        GameCleared += () =>
        {
            if (topLevel < currentLevel)
                topLevel++;

            if (clearUI != null) clearUI.gameObject.SetActive(true);

            clearUI.ShowResult(true);
        };
    }

    // 게임 종료 이벤트 호출
    public void InvokeGameEnd()
    {
        GameCleared?.Invoke();
    }

    // 메테오 이벤트 호출 (위치 기반)
    public void InvokeMeteor(Vector3 targetPos)
    {
        MeteorOccurred?.Invoke(targetPos);
    }

    public void RegisterBall(GameObject ball)
    {
        CurrentBall = ball;
    }

    public void UnregisterBall(GameObject ball)
    {
        if (CurrentBall == ball)
            CurrentBall = null;
    }

    public void SetLevel(int level)
    {
        currentLevel = level;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            // Launcher 위치 기준으로 메테오 발생
            var launcher = FindObjectOfType<LauncherBase>();
            if (launcher != null)
                InvokeMeteor(launcher.transform.position);
        }
        // Ball 속도 체크 및 처리
        if (CurrentBall != null)
        {
            Rigidbody2D rb = CurrentBall.GetComponent<Rigidbody2D>();
            if (rb != null && rb.velocity.magnitude <= 0.01f)
            {
                // Ball 위치에 Launcher 프리팹 생성 (y + 0.3f)
                if (launcherPrefab != null)
                {
                    Vector3 spawnPos = CurrentBall.transform.position;
                    spawnPos.y += 0.3f;
                    var launcherObj = Instantiate(launcherPrefab, spawnPos, Quaternion.identity);
                    if (launcherObj.TryGetComponent<DefaultLauncher>(out var defaultLauncher))
                    {
                        defaultLauncher.SetPowerCanvasCamera(GMHCamera);
                    }
                }

                Destroy(CurrentBall);
                CurrentBall = null;
            }
        }
    }
}
