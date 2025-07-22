using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class GameManager : Singleton<GameManager>
{
    public int TopLevel() => topLevel;

    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int topLevel = 0;

    // 1. 공이 골에 들어간 뒤의 게임 종료 C# 이벤트
    public event Action GameEnded;

    // 2. 골프의 "포어" 연출을 위한 메테오 발생 C# 이벤트
    public event Action MeteorOccurred;

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

    }

    // 게임 종료 이벤트를 호출하는 public 함수
    public void InvokeGameEnd()
    {
        GameEnded?.Invoke();
    }

    // 메테오 이벤트를 호출하는 public 함수
    public void InvokeMeteor()
    {
        MeteorOccurred?.Invoke();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            InvokeMeteor();
        }
    }
}
