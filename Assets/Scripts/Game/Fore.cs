using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Fore : MonoBehaviour
{
    [Header("Meteor 프리팹")]
    public GameObject meteorPrefab;
    [Header("Meteor 랜덤 생성 Y축 오프셋 (위에서 떨어질 경우 예시)")]
    public float spawnYOffset = 8f;

    private void OnEnable()
    {
        GameManager.Instance.MeteorOccurred += onMeteorOccurred;
    }

    private void OnDisable()
    {
        GameManager.Instance.MeteorOccurred -= onMeteorOccurred;
    }

    // Meteor 이벤트 발생 시, 호출 위치를 향해 Meteor 투사체를 생성/발사
    private void onMeteorOccurred(Vector3 targetPos)
    {
        UnityEngine.Debug.Log("Meteor event occurs!");

        if (meteorPrefab == null)
        {
            UnityEngine.Debug.LogWarning("Meteor Prefab이 Fore.cs에 연결되어 있지 않습니다!");
            return;
        }

        // 맵 밖(위쪽)에서 X축은 목표물과 비슷하게, Y축은 오프셋을 두고 생성
        Vector2 spawnPos = new Vector2(
            targetPos.x + UnityEngine.Random.Range(-2f, 2f), // 조금 좌/우 랜덤성
            targetPos.y + spawnYOffset           // 위쪽 오프셋
        );

        GameObject meteor = Instantiate(meteorPrefab, spawnPos, Quaternion.identity);

        // 방향/속도 계산 및 할당
        Vector2 dir = ((Vector2)targetPos - spawnPos).normalized;
        float speed = UnityEngine.Random.Range(0.5f, 1f);

        Rigidbody2D rb = meteor.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.velocity = dir * speed * 10f;

        // Meteor.cs에 타겟 위치, 방향 등 넘길 수도 있음
        Meteor meteorScript = meteor.GetComponent<Meteor>();
        if (meteorScript != null)
        {
            meteorScript.direction = dir;
            meteorScript.speed = speed;
        }
    }
}
