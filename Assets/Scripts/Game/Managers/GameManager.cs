using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Random = UnityEngine.Random;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int topLevel = 0;

    public int TopLevel() => topLevel;

    // 골 도달 이벤트
    public event Action GameCleared;

    // 메테오 이벤트 (위치 포함)
    public event Action<Vector3> MeteorOccurred;
    // 새 소환 위치 기반 이벤트
    public event Action<Vector3> BirdSpawnOccurred; 

    [SerializeField] public GameObject CurrentBall { get; private set; }

    [Header("런처 프리팹")]
    public GameObject launcherPrefab;

    [SerializeField] private ClearUI clearUI;

    public Camera GMHCamera;

    // 1. 전역 컨테이너 선언
    public static HashSet<string> stageSceneNames = new HashSet<string>();

    protected override void Awake()
    {
        base.Awake();
        // 2. 빌드에 포함된 Stage 씬 이름을 컨테이너에 등록
        RegisterStageScenes();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // 3. Stage 씬 자동 등록 메소드
    private void RegisterStageScenes()
    {
        stageSceneNames.Clear();
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < sceneCount; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = Path.GetFileNameWithoutExtension(path);
            // "Stage" + 정수 형태 검사
            if (sceneName.StartsWith("Stage"))
            {
                string numberPart = sceneName.Substring("Stage".Length);
                if (int.TryParse(numberPart, out _))
                {
                    stageSceneNames.Add(sceneName);
                }
            }
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (stageSceneNames.Contains(scene.name))
        {
            Debug.Log($"{scene.name} OnSceneLoaded!!!");

            clearUI = FindObjectOfType<ClearUI>();

            // 필요 시 로딩 시 처리할 로직 작성
            GameCleared += () =>
            {
                if (topLevel < currentLevel)
                    topLevel++;

                if (clearUI != null) clearUI.gameObject.SetActive(true);

                clearUI.ShowResult(true);
            };

            clearUI.gameObject.SetActive(false);
        }
    }



    // 게임 종료 이벤트 호출
    public void StageComplete()
    {
        GameCleared?.Invoke();
    }

    // 메테오 이벤트 호출 (위치 기반)
    public void InvokeMeteor(Vector3 targetPos)
    {
        MeteorOccurred?.Invoke(targetPos);
    }
    // 새(Bird) 소환 이벤트 호출
    public void InvokeBirdSpawn(Vector3 targetPos)
    {
        BirdSpawnOccurred?.Invoke(targetPos);
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

        if (Input.GetKeyDown(KeyCode.B))
        {
            var launcher = FindObjectOfType<LauncherBase>();
            if (launcher != null)
                InvokeBirdSpawn(launcher.transform.position);
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
                        // 50% 확률로 파워 속도 랜덤 설정
                        if (Random.value < 0.5f)
                        {
                            var aimInput = launcherObj.GetComponent<AimInputHandler>();
                            if (aimInput != null)
                                aimInput.SetPowerSpeed(Random.Range(0.2f, 3.0f));
                        }
                    }
                }

                Destroy(CurrentBall);
                CurrentBall = null;
            }
        }
    }
}
