using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeGoal : MonoBehaviour
{
    private Vector3? targetPos = null;

    public void TrollMove()
    {
        // 위 또는 아래로 랜덤하게 목표 위치 설정
        float direction = Random.value < 0.5f ? 1f : -1f;
        targetPos = transform.position + new Vector3(0, 10f * direction, 0);
        Debug.Log($"FakeGoal이 { (direction > 0 ? "위" : "아래") }로 이동 목표 설정!");
    }

    private void Update()
    {
        if (targetPos.HasValue)
        {
            // 목표 위치까지 3f 속도로 이동
            transform.position = Vector3.MoveTowards(transform.position, targetPos.Value, 5f * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPos.Value) < 0.01f)
            {
                transform.position = targetPos.Value;
                targetPos = null;
            }
        }
    }
}
