using UnityEngine;

public class Dispensor : MonoBehaviour
{
    public Launcher launcher; // Launcher ��ũ��Ʈ ����

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("����! ��ġ: " + transform.position);

        // Launcher ��ũ��Ʈ�� ���� ��ġ ����
        launcher.UpdateLaunchPoint(transform);

        // ���� �� ProjectileTracker ���� (���� ����)
        Destroy(this);
    }
}
