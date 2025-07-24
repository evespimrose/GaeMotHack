using System.Diagnostics;
using UnityEngine;

public class AimInputHandler : MonoBehaviour
{
    private IAimInputHandler handler;
    private enum ControlState { Power, Angle, None }
    private ControlState currentControlState = ControlState.None;

    // 파워 관련 변수
    [Header("Power Range")]
    [SerializeField] private float powerMin = 0f;
    [SerializeField] private float powerMax = 1f;
    private float power = 0f;
    private bool powerIncreasing = true;
    private bool hasReachedMax = false;
    private bool hasReachedMin = false;

    // 각도 관련 변수
    private float angle = 0f;
    private bool angleIncreasing = true;

    [Header("Control Speeds")]
    [SerializeField] private float aimSpeed = 120f;    // 초당 120도
    [SerializeField] private float powerSpeed = 1f;    // 초당 1.0

    [Header("Power Zones")]
    [Range(0f, 0.5f)] public float badRangeMin = 0f;
    [Range(0f, 0.5f)] public float badRangeMax = 0.3f;
    [Range(0f, 0.5f)] public float goodRangeMin = 0.5f;
    [Range(0f, 0.5f)] public float goodRangeMax = 0.6f;
    [Range(0f, 1f)] public float minimumPower = 0.1f; // 사용 안 할 경우 제거 가능

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
                handler.OnStartAiming(Vector2.zero);
            }
            else if (currentControlState == ControlState.Angle)
            {
                currentControlState = ControlState.Power;
                handler.OnStartPowerHandling();
                hasReachedMax = false;
                hasReachedMin = false;
            }
        }

        if (Input.GetKey(KeyCode.Space))
        {
            if (currentControlState == ControlState.Power)
            {
                UpdatePower();
            }
            else if (currentControlState == ControlState.Angle)
            {
                UpdateAngle();
            }
        }

        if (currentControlState == ControlState.Power && hasReachedMax && hasReachedMin)
        {
            float finalPower = minimumPower;
            handler.OnEndPowerHandling(finalPower);
            ResetControlState();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (currentControlState == ControlState.Power)
            {
                float finalPower = power;
                handler.OnEndPowerHandling(finalPower);
                ResetControlState();
            }
            else if (currentControlState == ControlState.Angle)
            {
                handler.OnEndAiming(new Vector2(angle, 0));
            }
        }
    }

    // 각도 조절(aim) 속도 적용
    void UpdateAngle()
    {
        float delta = aimSpeed * Time.deltaTime;
        if (angleIncreasing)
        {
            angle += delta;
            if (angle >= 180f)
            {
                angle = 180f;
                angleIncreasing = false;
            }
        }
        else
        {
            angle -= delta;
            if (angle <= 0f)
            {
                angle = 0f;
                angleIncreasing = true;
            }
        }
        angle = Mathf.Clamp(angle, 0f, 180f);
    }

    // 파워(power) 게이지 속도 적용 (PowerMin/PowerMax 반영)
    void UpdatePower()
    {
        float delta = powerSpeed * Time.deltaTime;
        if (powerIncreasing)
        {
            power += delta;
            if (power >= powerMax)
            {
                power = powerMax;
                powerIncreasing = false;
                hasReachedMax = true;
            }
        }
        else
        {
            power -= delta;
            if (power <= powerMin)
            {
                power = powerMin;
                powerIncreasing = true;
                hasReachedMin = true;
            }
        }
        power = Mathf.Clamp(power, powerMin, powerMax);
    }

    void ResetControlState()
    {
        currentControlState = ControlState.None;
        power = powerMin; // 항상 최소값에서 시작
        powerIncreasing = true;
        hasReachedMax = false;
        hasReachedMin = false;
        angle = 0f;
        angleIncreasing = true;
    }

    // 외부에서 호출 가능한 public Setter 함수
    public void SetAimSpeed(float newSpeed)
    {
        aimSpeed = newSpeed;
    }
    public void SetPowerSpeed(float newSpeed)
    {
        powerSpeed = newSpeed;
    }

    // 외부에서 값 읽기용
    public float GetPower() => power;
    public float GetAngle() => angle;
    public string GetCurrentControlState() => currentControlState.ToString();

    // 추가: 현재 파워의 노멀라이즈 값 (UI 등에서 사용 가능)
    public float GetNormalizedPower() => (power - powerMin) / (powerMax - powerMin);
}
