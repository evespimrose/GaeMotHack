using System.Diagnostics;
using UnityEngine;

public class AimInputHandler : MonoBehaviour
{
    private IAimInputHandler handler;
    private enum ControlState { Power, Angle, None }
    private ControlState currentControlState = ControlState.None;

    // 파워 관련 변수
    private float power = 0f;
    private bool powerIncreasing = true;
    private bool hasReachedMax = false;
    private bool hasReachedMin = false;

    // 각도 관련 변수
    private float angle = 0f;
    private bool angleIncreasing = true;

    [Header("Control Speeds")]
    [SerializeField] private float aimSpeed = 120f;    // 초당 120도 (Inspector/코드 모두에서 제어)
    [SerializeField] private float powerSpeed = 1f;    // 초당 1.0 (Inspector/코드 모두에서 제어)

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
        angle = Mathf.Clamp(angle, 0f, 180f); // 0~180도 제한
    }

    // 파워(power) 게이지 속도 적용
    void UpdatePower()
    {
        float delta = powerSpeed * Time.deltaTime;
        if (powerIncreasing)
        {
            power += delta;
            if (power >= 1f)
            {
                power = 1f;
                powerIncreasing = false;
                hasReachedMax = true;
            }
        }
        else
        {
            power -= delta;
            if (power <= 0f)
            {
                power = 0f;
                powerIncreasing = true;
                hasReachedMin = true;
            }
        }
        power = Mathf.Clamp01(power);
    }

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


    // ⭐️ 외부에서 호출 가능한 public Setter 함수
    public void SetAimSpeed(float newSpeed)
    {
        aimSpeed = newSpeed;
    }
    public void SetPowerSpeed(float newSpeed)
    {
        powerSpeed = newSpeed;
    }


    // ⭐️ 프로퍼티 형태 사용 (혹시 몰라서 추가 해뒀음)
    //public float AimSpeed
    //{
    //    get => aimSpeed;
    //    set => aimSpeed = value;
    //}
    //public float PowerSpeed
    //{
    //    get => powerSpeed;
    //    set => powerSpeed = value;
    //}

    // 외부에서 값 읽기용
    public float GetPower() => power;
    public float GetAngle() => angle;
    public string GetCurrentControlState() => currentControlState.ToString();
}
