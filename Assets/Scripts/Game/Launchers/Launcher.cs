using UnityEngine;

public class Launcher : MonoBehaviour
{
    public Rigidbody2D projectilePrefab; // �߻�ü ������
    public Transform initialLaunchPoint; // �ʱ� �߻� ��ġ
    public float launchForceMultiplier = 10f; // �߻� ���� ���
    public float maxLaunchForce = 10f; // �ִ� �߻� ���� ����

    private bool isAiming = false;
    private Vector2 startPosition;
    private Transform currentLaunchPoint; // ���� �߻� ��ġ

    void Start()
    {
        // �ʱ� �߻� ��ġ ����
        currentLaunchPoint = initialLaunchPoint;
    }

    void Update()
    {
        // ���콺 ���� ��ư�� ������ ��
        if (Input.GetMouseButtonDown(0))
        {
            StartAiming();
        }

        // ���콺 ���� ��ư�� ������ ��
        if (Input.GetMouseButtonUp(0) && isAiming)
        {
            FireProjectile();
        }
    }

    // �߻� �غ� ����
    private void StartAiming()
    {
        isAiming = true;
        startPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log("�߻� �غ� ����");
    }

    // �߻�ü �߻�
    private void FireProjectile()
    {
        isAiming = false;
        Vector2 endPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 launchVector = startPosition - endPosition; // �巡�� ���� ����

        float angle = Mathf.Atan2(launchVector.y, launchVector.x) * Mathf.Rad2Deg; // �߻� ���� ���
        float forceMagnitude = Mathf.Clamp(launchVector.magnitude, 0, maxLaunchForce); // �߻� ���� ���� �� ���
        float normalizedForce = forceMagnitude / maxLaunchForce; // ����ȭ�� �߻� ����

        // �߻� ��� �޼ҵ� ȣ��
        Release(angle, normalizedForce);
    }

    // �߻� ��� �޼ҵ�
    private void Release(float angle, float force)
    {
        Debug.Log("Release() ȣ��� - ����: " + angle + ", ��: " + force);

        // �ӽ� �߻� ���� (���� ��� �� ���� �߻� �������� ��ü ����)
        Rigidbody2D projectileInstance = Instantiate(projectilePrefab, currentLaunchPoint.position, Quaternion.identity);
        projectileInstance.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        Vector2 forceVector = projectileInstance.transform.right * force * launchForceMultiplier;
        projectileInstance.AddForce(forceVector, ForceMode2D.Impulse);

        // �߻� �� ���� ��ġ ������ ���� �ݹ� �Լ� ���
        Dispensor tracker = projectileInstance.gameObject.AddComponent<Dispensor>();
        tracker.launcher = this; // Launcher ��ũ��Ʈ ���� ����
    }

    // �߻� ��ġ ������Ʈ
    public void UpdateLaunchPoint(Transform newLaunchPoint)
    {
        currentLaunchPoint = newLaunchPoint;
        Debug.Log("�߻� ��ġ ������Ʈ: " + currentLaunchPoint.position);
    }
}
