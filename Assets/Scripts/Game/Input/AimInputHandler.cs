using UnityEngine;

public class AimInputHandler : MonoBehaviour
{
    private IAimInputHandler handler;
    private bool isAiming = false;
    private Vector2 startPosition;
    private Vector2 currentPosition;

    private float gizmoDisplayTimer = 0f;
    private const float GIZMO_DURATION = 3f;

    private void Awake()
    {
        handler = GetComponent<IAimInputHandler>();
        if (handler == null)
            Debug.LogError("IAimInputHandler를 구현한 컴포넌트가 필요합니다.");
    }

    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
        HandleMouseInput();
#endif
#if UNITY_ANDROID || UNITY_IOS
        HandleTouchInput();
#endif

        if (gizmoDisplayTimer > 0f)
            gizmoDisplayTimer -= Time.deltaTime;
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isAiming = true;
            startPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            handler.OnStartAiming(startPosition);
        }

        if (isAiming)
        {
            currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            handler.OnUpdateAiming(currentPosition);
        }

        if (Input.GetMouseButtonUp(0) && isAiming)
        {
            isAiming = false;
            currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            handler.OnEndAiming(currentPosition);
            gizmoDisplayTimer = GIZMO_DURATION;
        }
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(touch.position);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                isAiming = true;
                startPosition = worldPos;
                handler.OnStartAiming(startPosition);
                break;

            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                if (isAiming)
                {
                    currentPosition = worldPos;
                    handler.OnUpdateAiming(currentPosition);
                }
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                if (isAiming)
                {
                    isAiming = false;
                    currentPosition = worldPos;
                    handler.OnEndAiming(currentPosition);
                    gizmoDisplayTimer = GIZMO_DURATION;
                }
                break;
        }
    }

    private void OnDrawGizmos()
    {
        if (isAiming || gizmoDisplayTimer > 0f)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(startPosition, 0.1f);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(currentPosition, 0.1f);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(startPosition, currentPosition);
        }
    }
}
