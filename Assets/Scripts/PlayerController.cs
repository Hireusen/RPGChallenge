using UnityEngine;

/// <summary>
/// 플레이어 오브젝트에 부착하는 C# 스크립트입니다.
/// 입력은 InputManager 이벤트를 구독하여 받습니다.
/// </summary>
public class PlayerController : MonoBehaviour
{
    #region ─────────────────────────▶ 인스펙터 ◀─────────────────────────
    [Header("필수 참조 연결")]
    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Transform _cameraTr;

    [Header("이동 설정")]
    [SerializeField] private float _walkSpeed = 5f;
    [SerializeField] private float _runSpeed = 8f;
    [SerializeField] private float _guardSpeedMultiflier = 0.4f;
    [SerializeField] private float _rotateSharpness = 15f;

    [Header("점프 설정")]
    [SerializeField] private float _jumpForce = 7f;
    [SerializeField] private float _doubleJumpForce = 6f;
    [SerializeField] private float _jumpDelay = 0.5f;

    [Header("바닥 감지")]
    [SerializeField] private float _groundUpStick = 0.2f;
    [SerializeField] private float _groundDownStick = 0.25f;
    [SerializeField] private LayerMask _groundLayer;

    [Header("애니메이터 파라미터")]
    [SerializeField] private string _paramSpeed = "aSpeed";
    [SerializeField] private string _paramRun = "bRun";
    [SerializeField] private string _paramJump = "tJump";
    [SerializeField] private string _paramAttack = "tAttack";
    [SerializeField] private string _paramGuard = "bGuard";

    [Header("애니메이터 튜닝")]
    [SerializeField] private float _speedDamp = 0.12f;

    [Header("디버그")]
    [SerializeField] private bool _log = false;
    [SerializeField] private bool _ray = false;
    #endregion

    #region ─────────────────────────▶ 내부 변수 ◀─────────────────────────
    // 입력 캐싱
    private Vector2 _inputMove;
    private bool _inputRun;
    private bool _inputJump;
    private bool _inputAttack;
    private bool _inputGuard;
    // 점프
    private bool _isGrounded;
    private bool _canDoubleJump;
    private float _nextJumpEnableTime;
    // 애니메이터 해시
    private int _hashSpeed, _hashRun, _hashJump, _hashAttack, _hashGuard;
    private bool _hasRunParam, _hasJumpParam, _hasAttackParam, _hasGuardParam;
    // 바인딩 여부
    private bool _isBound;
    #endregion

    #region ─────────────────────────▶ 바인드 & 핸들러 ◀─────────────────────────
    private void TryBindInput()
    {
        if (_isBound) {
            return;
        }
        var im = InputManager.Ins;
        if (im == null || !im.IsReady) {
            return;
        }
        im.OnMove += HandleMove;
        im.OnRun += HandleRun;
        im.OnJump += HandleJump;
        im.OnAttack += HandleAttack;
        im.OnGuard += HandleGuard;
        _isBound = true;
        if (_log) {
            De.Print($"플레이어 컨트롤러에서 바인드를 완료했습니다.");
        }
    }

    private void UnBindInput()
    {
        if (!_isBound) {
            return;
        }
        var im = InputManager.Ins;
        if (im == null || !im.IsReady) {
            return;
        }
        im.OnMove -= HandleMove;
        im.OnRun -= HandleRun;
        im.OnJump -= HandleJump;
        im.OnAttack -= HandleAttack;
        im.OnGuard -= HandleGuard;
        _isBound = false;
        if (_log) {
            De.Print($"플레이어 컨트롤러에서 언바인드를 완료했습니다.");
        }
    }

    private void HandleMove(Vector2 v) => _inputMove = v;
    // True → KeyDown / False → KeyUp
    private void HandleRun(bool pressed) => _inputRun = pressed;
    private void HandleAttack(bool pressed) => _inputAttack = pressed;
    private void HandleGuard(bool pressed) => _inputGuard = pressed;
    private void HandleJump(bool pressed) => _inputJump = pressed;
    #endregion

    private void TickGroundCheck()
    {
        Vector3 origin = transform.position + Vector3.up * _groundUpStick;
        _isGrounded = Physics.Raycast(origin, Vector3.down, _groundDownStick, _groundLayer);
        if (_ray) {
            Color color = _isGrounded ? Color.green : Color.red;
            Debug.DrawRay(origin, Vector3.down * _groundDownStick, color, Time.deltaTime);
        }
    }

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    // Y 값이 제거된 순수 방향을 빌드
    private Vector3 BuildMoveDirection(Vector3 input)
    {
        // 절대적인 정면 방향
        Vector3 camForward = Vector3.ProjectOnPlane(_cameraTr.forward, Vector3.up).normalized;
        // 절대적인 오른쪽 방향
        Vector3 camRight = Vector3.ProjectOnPlane(_cameraTr.right, Vector3.up).normalized;
        // 정면 * 앞뒤 입력값 + 우측 * 좌우 입력값
        return (camForward * input.z + camRight * input.x).normalized;
    }

    private void TickMove(Vector3 moveDir)
    {
        // 입력이 없다면
        if (moveDir == Vector3.zero) {
            _rb.velocity = new Vector3(0f, _rb.velocity.y, 0f);
            return;
        }
        // 속도 결정
        float speed = _inputRun ? _runSpeed : _walkSpeed;
        if (_inputGuard) {
            speed = _guardSpeedMultiflier;
        }
        // 속도 직접 설정
        Vector3 vel = moveDir * speed;
        vel.y = _rb.velocity.y;
        _rb.velocity = vel;
    }

    private void TickRotate(Vector3 moveDir)
    {
        // 입력이 없다면
        if (moveDir == Vector3.zero) {
            return;
        }
        // 바라보아야 할 방향
        Quaternion desiredRot = Quaternion.LookRotation(moveDir, Vector3.up);
        float t = UMath.GetSmoothT(_rotateSharpness, Time.fixedDeltaTime);
        // 구형 보간
        Quaternion rotation = Quaternion.Slerp(transform.rotation, desiredRot, t);
        _rb.MoveRotation(rotation);
    }

    private void RunJump(float force)
    {
        _rb.AddForce(Vector3.up * force, ForceMode.Impulse);
        _nextJumpEnableTime = Time.fixedTime + _jumpDelay;
        if (_log) {
            De.Print("점프했습니다.");
        }
    }

    private bool TryGroundJump()
    {
        // 키 입력 필요 / 지상이어야 함 / 점프 쿨타임
        if (!_inputJump || !_isGrounded || Time.fixedTime < _nextJumpEnableTime) {
            return false;
        }
        // 점프
        RunJump(_jumpForce);
        _canDoubleJump = true;
        return true;
    }

    private bool TryAirJump()
    {
        // 키 입력 필요 / 공중이어야 함 / 더블 점프 가능 / 점프 쿨타임
        if (!_inputJump || _isGrounded || !_canDoubleJump || Time.fixedTime < _nextJumpEnableTime) {
            return false;
        }
        // 점프
        RunJump(_doubleJumpForce);
        _canDoubleJump = false;
        return true;
    }

    private void TickAnimator(Vector3 moveDir, bool jumped)
    {
        // 매 프레임 상태에 따라 Speed 파라미터 설정
        // Idle = 0, Walk = 0.5, Run = 1
        float speed01 = moveDir.magnitude * (_inputRun ? 1f : 0.5f);
        _animator.SetFloat(_hashSpeed, speed01, _speedDamp, Time.deltaTime);
        // 달리는 상태
        if (_hasRunParam) {
            _animator.SetBool(_hashRun, _inputRun);
        }
        // 점프 트리거
        if (_hasJumpParam && jumped) {
            _animator.SetTrigger(_hashJump);
        }
        // 공격 트리거
        if (_hasAttackParam && _inputAttack) {
            _animator.SetTrigger(_hashAttack);
        }
        // 가드 상태
        if (_hasGuardParam) {
            _animator.SetBool(_hashGuard, _inputGuard);
        }
    }
    #endregion

    #region ─────────────────────────▶ 초기화 함수 ◀─────────────────────────
    // 참조 자동 연결 및 성공 여부 반환
    private bool RefAutoConnect()
    {
        bool success = true;
        if (_rb == null) {
            _rb = GetComponent<Rigidbody>();
            if (_rb == null) success = false;
        }
        if (_animator == null) {
            _animator = GetComponent<Animator>();
            if (_animator == null) success = false;
        }
        if (_cameraTr == null && Camera.main != null) {
            _cameraTr = Camera.main.transform;
            if (_cameraTr == null) success = false;
        }
        return success;
    }

    private void ResetInputCache()
    {
        _inputMove = Vector2.zero;
        _inputRun = false;
        _inputJump = false;
        _inputAttack = false;
        _inputGuard = false;
        if (_log) {
            De.Print("플레이어 컨트롤러 : Input 캐시를 청소했습니다.");
        }
    }

    private void InitAnimatorHash()
    {
        _hashSpeed = Animator.StringToHash(_paramSpeed);
        _hasRunParam = !string.IsNullOrEmpty(_paramRun);
        if (_hasRunParam) {
            _hashRun = Animator.StringToHash(_paramRun);
        }
        _hasJumpParam = !string.IsNullOrEmpty(_paramJump);
        if (_hasJumpParam) {
            _hashJump = Animator.StringToHash(_paramJump);
        }
        _hasAttackParam = !string.IsNullOrEmpty(_paramAttack);
        if (_hasAttackParam) {
            _hashAttack = Animator.StringToHash(_paramAttack);
        }
        _hasGuardParam = !string.IsNullOrEmpty(_paramGuard);
        if (_hasGuardParam) {
            _hashGuard = Animator.StringToHash(_paramGuard);
        }
        if (_log) {
            De.Print("플레이어 컨트롤러 : 애니메이터 해시와 파라미터를 빌드했습니다.");
        }
    }
    #endregion

    #region ─────────────────────────▶ 메시지 함수 ◀─────────────────────────
    private void Awake()
    {
        if (!RefAutoConnect()) {
            De.Print("필수 참조가 연결되지 않았습니다. 확인해주세요.", LogType.Assert);
            enabled = false;
            return;
        }
        InitAnimatorHash();
        TryBindInput();
        _nextJumpEnableTime = Time.fixedTime - _jumpDelay;
    }

    private void OnEnable()
    {
        TryBindInput();
    }

    private void OnDisable()
    {
        ResetInputCache();
    }

    private void OnDestroy()
    {
        UnBindInput();
    }

    private void FixedUpdate()
    {
        // 방어 코드
        if (!RefAutoConnect()) {
            De.Print("필수 참조가 연결되지 않았습니다. 확인해주세요.", LogType.Assert);
            enabled = false;
            return;
        }
        // 바닥 여부 갱신
        TickGroundCheck(); 
        // 이동 방향 빌드
        Vector3 input = new Vector3(_inputMove.x, 0f, _inputMove.y);
        input = Vector3.ClampMagnitude(input, 1f);
        Vector3 moveDir = input.sqrMagnitude > 0.0001f ? BuildMoveDirection(input) : Vector3.zero;
        // 이동 및 점프 시도
        TickMove(moveDir);
        TickRotate(moveDir);
        bool groundJumped = TryGroundJump();
        bool airJumped = TryAirJump();
        // 애니메이션
        TickAnimator(moveDir, groundJumped || airJumped);
    }
    #endregion
}
