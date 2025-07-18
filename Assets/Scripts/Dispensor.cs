using System.Diagnostics;
using UnityEngine;

public class ProjectileTracker : MonoBehaviour
{
    public Launcher launcher; // Launcher 스크립트 참조

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("착지! 위치: " + transform.position);

        // Launcher 스크립트에 착지 위치 전달
        launcher.UpdateLaunchPoint(transform);

        // 착지 후 ProjectileTracker 제거 (선택 사항)
        Destroy(this);
    }
}
