using UnityEngine;

public class BirdSpawner : MonoBehaviour
{
    [Header("Bird 프리팹")]
    public GameObject birdPrefab;

    [Header("스폰 위치(맵 밖)")]
    public float spawnX = 12f; // 현재는 발사대 시작 위치보다 왼쪽
    public Vector2 spawnYRange = new Vector2(-2f, 5f); // 랜덤 스폰 영역(y)

    [Header("이동 속도/방향")]
    public Vector2 flyDirection = Vector2.right;
    public float flySpeed = 4f;

    void Update()
    {
        // B키 누를 때 새 생성
        if (Input.GetKeyDown(KeyCode.B))
        {
            SpawnBird();
        }
    }

    public void SpawnBird()
    {
        if (birdPrefab == null)
        {
            UnityEngine.Debug.LogWarning("Bird 프리팹이 연결되어 있지 않습니다!");
            return;
        }
        float randY = Random.Range(spawnYRange.x, spawnYRange.y);
        Vector2 spawnPos = new Vector2(spawnX, randY);

        GameObject bird = Instantiate(birdPrefab, spawnPos, Quaternion.identity);
        Rigidbody2D rb = bird.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = flyDirection.normalized * flySpeed;
        }
        else
        {
            UnityEngine.Debug.LogWarning("Bird에 Rigidbody2D가 없습니다!");
        }
    }
}
