using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GMHCamera : MonoBehaviour
{
    public float followSpeed = 5f;
    public float defaultZoom = 5f;
    public float zoomOutSize = 8f;
    private Camera cam;
    private Transform target;
    private bool isZoomedOut = false;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main;
        cam.orthographicSize = defaultZoom;
    }

    void LateUpdate()
    {
        // 타겟 갱신
        if (GameManager.Instance.CurrentBall != null)
        {
            target = GameManager.Instance.CurrentBall.transform;
        }
        else
        {
            var launcher = FindObjectOfType<LauncherBase>();
            if (launcher != null)
                target = launcher.transform;
        }
        // 따라가기
        if (target != null)
        {
            Vector3 targetPos = target.position;
            targetPos.z = transform.position.z;
            transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
        }
    }

    public void SetZoomOut(bool zoomOut)
    {
        Debug.Log($"SetZoomOut : {zoomOut}");

        if (zoomOut && !isZoomedOut)
        {
            cam.orthographicSize = zoomOutSize;
            isZoomedOut = true;
        }
        else if (!zoomOut && isZoomedOut)
        {
            cam.orthographicSize = defaultZoom;
            isZoomedOut = false;
        }
    }
}
