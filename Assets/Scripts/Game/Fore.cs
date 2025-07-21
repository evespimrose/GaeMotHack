using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fore : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.Instance.MeteorOccurred += OnMeteorOccurred;
    }

    private void OnDisable()
    {
        GameManager.Instance.MeteorOccurred -= OnMeteorOccurred;
    }

    private void OnMeteorOccurred()
    {
        // 메테오 이벤트 발생 시 실행할 코드 작성
        Debug.Log("Meteor 이벤트 발생!");
    }
}
