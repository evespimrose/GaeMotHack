using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    [HideInInspector] public Vector2 direction;
    [HideInInspector] public float speed;

    private void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.velocity = direction * speed * 10f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Launcher"))
        {
            var launcher = collision.collider.GetComponent<LauncherBase>();
            if (launcher != null)
            {
                if (launcher.IsPowerHandlingOnly())
                {
                    // �Ŀ� ���� ��: ������ ����ڰ� ������ ���� + ���� �Ŀ�
                    float curPower = 0.1f;
                    float angle = 0f;

                    if (launcher is DefaultLauncher dl)
                    {
                        curPower = Mathf.Max(dl.CurrentPower, 0.1f);
                        angle = dl.CurrentAngle;
                    }
                    float rad = angle * Mathf.Deg2Rad;
                    Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

                    launcher.StunAndForceShoot(dir, curPower);
                }
                else if (launcher.IsAngleHandlingOnly())
                {
                    // ���� ���� ��: ���� ���� ����, �Ŀ� 0.1
                    float angle = 0f;
                    if (launcher is DefaultLauncher dl)
                        angle = dl.CurrentAngle;
                    float rad = angle * Mathf.Deg2Rad;
                    Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
                    launcher.StunAndForceShoot(dir, 0.1f);
                }
                else
                {
                    // �� �� �ƴ�: ���⡤�Ŀ� ����
                    float randAngle = UnityEngine.Random.Range(0f, 180f);
                    float radian = randAngle * Mathf.Deg2Rad;
                    Vector2 randomDir = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
                    float randPower = UnityEngine.Random.Range(0.1f, 1.0f);
                    launcher.StunAndForceShoot(randomDir, randPower);
                }
            }
            Destroy(gameObject);
        }
    }

}
