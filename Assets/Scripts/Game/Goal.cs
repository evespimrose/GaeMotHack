using UnityEngine;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 충돌한 오브젝트가 Ball 컴포넌트를 가지고 있는지 체크
        // 게임 성공 처리 로직 호출
        //GameManager.Instance?.OnLevelComplete();
    }
}
