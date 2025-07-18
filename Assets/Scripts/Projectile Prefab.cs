using System.Diagnostics;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    public Rigidbody2D projectilePrefab; // 발사체 프리팹
    public Transform initialLaunchPoint; // 초기 발사 위치
    public float launchForceMultiplier = 10f; // 발사 강도 계수
    public float maxLaunchForce = 10f; // 최대 발사 강도 제한

    private bool isAiming = false;
    private Vector2 startPosition;
    private Transform currentLaunchPoint; // 현재 발사 위치

    void Start()
    {
        // 초기 발사 위치 설정
        currentLaunchPoint = initialLaunchPoint;
    }

    void Update()
    {
        // 마우스 왼쪽 버튼을 눌렀을 때
        if (Input.GetMouseButtonDown(0))
        {
            StartAiming();
        }

        // 마우스 왼쪽 버튼을 떼었을 때
        if (Input.GetMouseButtonUp(0) && isAiming)
        {
            FireProjectile();
        }
    }

    // 발사 준비 시작
    private void StartAiming()
    {
        isAiming = true;
        startPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log("발사 준비 시작");
    }

    // 발사체 발사
    private void FireProjectile()
    {
        isAiming = false;
        Vector2 endPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 launchVector = startPosition - endPosition; // 드래그 방향 벡터

        float angle = Mathf.Atan2(launchVector.y, launchVector.x) * Mathf.Rad2Deg; // 발사 각도 계산
        float forceMagnitude = Mathf.Clamp(launchVector.magnitude, 0, maxLaunchForce); // 발사 강도 제한 및 계산
        float normalizedForce = forceMagnitude / maxLaunchForce; // 정규화된 발사 강도

        // 발사 명령 메소드 호출
        Release(angle, normalizedForce);
    }

    // 발사 명령 메소드
    private void Release(float angle, float force)
    {
        Debug.Log("Release() 호출됨 - 각도: " + angle + ", 힘: " + force);

        // 임시 발사 로직 (궤적 계산 후 실제 발사 로직으로 대체 예정)
        Rigidbody2D projectileInstance = Instantiate(projectilePrefab, currentLaunchPoint.position, Quaternion.identity);
        projectileInstance.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        Vector2 forceVector = projectileInstance.transform.right * force * launchForceMultiplier;
        projectileInstance.AddForce(forceVector, ForceMode2D.Impulse);

        // 발사 후 착지 위치 추적을 위해 콜백 함수 등록
        ProjectileTracker tracker = projectileInstance.gameObject.AddComponent<ProjectileTracker>();
        tracker.launcher = this; // Launcher 스크립트 참조 전달
    }

    // 발사 위치 업데이트
    public void UpdateLaunchPoint(Transform newLaunchPoint)
    {
        currentLaunchPoint = newLaunchPoint;
        Debug.Log("발사 위치 업데이트: " + currentLaunchPoint.position);
    }
}
