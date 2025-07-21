using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    // 1. 공이 골에 들어간 뒤의 게임 종료 C# 이벤트
    public event System.Action GameEnded;

    // 2. 골프의 "포어" 연출을 위한 메테오 발생 C# 이벤트
    public event System.Action MeteorOccurred;

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
