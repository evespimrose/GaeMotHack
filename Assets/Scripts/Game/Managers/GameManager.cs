using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 싱글턴 구조 예시 (최소 구현)
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // 1. 골 도달 이벤트
    public event System.Action GameEnded;

    // 2. Meteor(유성) 발생 이벤트
    public event System.Action<Vector3> MeteorOccurred; // <--- Meteor 생성 위치 등 전달 가능

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 게임 종료 이벤트 호출
    public void InvokeGameEnd()
    {
        GameEnded?.Invoke();
    }

    // Meteo 이벤트 호출, 호출 위치로부터 발사
    public void InvokeMeteor(Vector3 targetPos)
    {
        MeteorOccurred?.Invoke(targetPos);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            // 예시: Launcher 위치로 떨어트림
            var launcher = FindObjectOfType<LauncherBase>();
            if (launcher != null)
                InvokeMeteor(launcher.transform.position);
        }
    }
}
