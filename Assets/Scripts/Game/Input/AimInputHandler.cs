using UnityEngine;

public class AimInputHandler : MonoBehaviour
{
    private IAimInputHandler handler;
    private enum ControlState { Power, Angle, None }
    private ControlState currentControlState = ControlState.None;

    // 파워 조절 관련 변수
    private float power = 0f;
    private bool powerIncreasing = true;
    private bool hasReachedMax = false; // 최대값 도달 여부
    private bool hasReachedMin = false; // 최소값 도달 여부

    // 각도 조절 관련 변수
    private float angle = 0f;
    private bool angleIncreasing = true;

    [Header("Power Range")]
    [Range(0f, 0.5f)] public float badRangeMin = 0f;
    [Range(0f, 0.5f)] public float badRangeMax = 0.3f;
    [Range(0f, 0.5f)] public float goodRangeMin = 0.5f;
    [Range(0f, 0.5f)] public float goodRangeMax = 0.6f;
    [Range(0f, 1f)] public float minimumPower = 0.1f;

    void Awake()
    {
        handler = GetComponent<IAimInputHandler>();
        if (handler == null)
            UnityEngine.Debug.LogError("IAimInputHandler를 구현한 컴포넌트가 필요합니다.");
    }

    void Update()
    {
        HandleSpacebarInput();
    }

    void HandleSpacebarInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentControlState == ControlState.None)
            {
                currentControlState = ControlState.Angle;
                handler.OnStartAiming(Vector2.zero); // 각도 조절 시작
            }
            else if (currentControlState == ControlState.Angle)
            {
                currentControlState = ControlState.Power;
                handler.OnStartPowerHandling(); // 파워 조절 시작
                hasReachedMax = false; // 초기화
                hasReachedMin = false; // 초기화
            }
        }

        if (Input.GetKey(KeyCode.Space))
        {
            if (currentControlState == ControlState.Power)
            {
                UpdatePower(); // 파워 업데이트
            }
            else if (currentControlState == ControlState.Angle)
            {
                UpdateAngle(); // 각도 업데이트
            }
        }

        // 게이지 바가 1회 왕복을 마치면 최소 파워로 설정
        if (currentControlState == ControlState.Power && hasReachedMax && hasReachedMin)
        {
            float finalPower = minimumPower; // 최소 파워로 설정
            handler.OnEndPowerHandling(finalPower); // 파워 조절 종료
            ResetControlState(); // 상태 초기화
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (currentControlState == ControlState.Power)
            {
                float finalPower = power;
                handler.OnEndPowerHandling(finalPower); // 파워 조절 종료
                ResetControlState(); // 상태 초기화
            }
            else if (currentControlState == ControlState.Angle)
            {
                handler.OnEndAiming(new Vector2(angle, 0)); // 각도 조절 종료 (각도 전달)
            }
        }
    }

    void UpdatePower()
    {
        if (powerIncreasing)
        {
            power += Time.deltaTime;
            if (power >= 1f)
            {
                power = 1f;
                powerIncreasing = false;
                hasReachedMax = true; // 최대값 도달
            }
        }
        else
        {
            power -= Time.deltaTime;
            if (power <= 0f)
            {
                power = 0f;
                powerIncreasing = true;
                hasReachedMin = true; // 최소값 도달
            }
        }
        power = Mathf.Clamp01(power); // 0과 1 사이로 제한
    }

    void UpdateAngle()
    {
        if (angleIncreasing)
        {
            angle += Time.deltaTime;
            if (angle >= 1f)
            {
                angle = 1f;
                angleIncreasing = false;
            }
        }
        else
        {
            angle -= Time.deltaTime;
            if (angle <= 0f)
            {
                angle = 0f;
                angleIncreasing = true;
            }
        }
        angle = Mathf.Clamp01(angle); // 0과 1 사이로 제한
    }

    // 상태 초기화
    void ResetControlState()
    {
        currentControlState = ControlState.None;
        power = 0f;
        powerIncreasing = true;
        hasReachedMax = false;
        hasReachedMin = false;
        angle = 0f;
        angleIncreasing = true;
    }

    public float GetPower()
    {
        return power;
    }

    public float GetAngle()
    {
        return angle * 180f;
    }

    public string GetCurrentControlState()
    {
        return currentControlState.ToString();
    }
}
