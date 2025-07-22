using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int topLevel = 0;

    public int TopLevel() => topLevel;

    // 골 도달 이벤트
    public event Action GameEnded;

    // 메테오 이벤트 (위치 포함)
    public event Action<Vector3> MeteorOccurred;

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
    }

    // 게임 종료 이벤트 호출
    public void InvokeGameEnd()
    {
        GameEnded?.Invoke();
    }

    // 메테오 이벤트 호출 (위치 기반)
    public void InvokeMeteor(Vector3 targetPos)
    {
        MeteorOccurred?.Invoke(targetPos);
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
    }
}
