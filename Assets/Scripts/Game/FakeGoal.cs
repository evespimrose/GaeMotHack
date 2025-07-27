using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeGoal : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TrollMove()
    {
        // 위 또는 아래로 랜덤하게 이동
        float direction = Random.value < 0.5f ? 1f : -1f;
        Vector3 newPos = transform.position + new Vector3(0, 10f * direction, 0);
        transform.position = newPos;
        Debug.Log($"FakeGoal이 { (direction > 0 ? "위" : "아래") }로 이동!");
    }
}
