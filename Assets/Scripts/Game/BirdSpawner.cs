using UnityEngine;

public class BirdSpawner : MonoBehaviour
{
    [Header("Bird ������")]
    public GameObject birdPrefab;

    [Header("���� ��ġ(�� ��)")]
    public float spawnX = 12f; // ����� �߻�� ���� ��ġ���� ����
    public Vector2 spawnYRange = new Vector2(-2f, 5f); // ���� ���� ����(y)

    [Header("�̵� �ӵ�/����")]
    public Vector2 flyDirection = Vector2.right;
    public float flySpeed = 4f;

    void Update()
    {
        // BŰ ���� �� �� ����
        if (Input.GetKeyDown(KeyCode.B))
        {
            SpawnBird();
        }
    }

    public void SpawnBird()
    {
        if (birdPrefab == null)
        {
            UnityEngine.Debug.LogWarning("Bird �������� ����Ǿ� ���� �ʽ��ϴ�!");
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
        // isRed 20% 확률로 true
        var birdScript = bird.GetComponent<Bird>();
        if (birdScript != null)
        {
            birdScript.isRed = (Random.value < 0.2f);
        }
    }
}
