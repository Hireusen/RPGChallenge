using System.Text;
using UnityEngine;

/// <summary>
/// 키 입력을 전달받아서 데이터와 함께 상태 머신에 전달한다.
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Transform))]
public class PlayerController : MonoBehaviour
{
    #region ─────────────────────────▶ 인스펙터 ◀─────────────────────────
    [Header("참조 연결")]
    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Transform _tr;
    [SerializeField] private Transform _cameraTr;
    [SerializeField] private EntityHeart _heart;

    [Header("바닥 판정")]
    [SerializeField] private float _groundUpStick = 0.2f;
    [SerializeField] private float _groundDownStick = 0.25f;
    [SerializeField] private LayerMask _groundLayer;

    [Header("피격/낙하 설정")]
    [SerializeField] private float _hitCooldown = 0.1f;
    [SerializeField] private float _dropImpactCooldown = 1f;
    [SerializeField] private float _dropImpactCondVelocity = -5f;

    [Header("디버그")]
    [SerializeField] private bool _log = false;
    [SerializeField] private bool _ray = false;

    [Header("상태 머신")]
    [SerializeField] private PlayerStateMachine _fsm; // 자체 생성
    #endregion

    #region ─────────────────────────▶ 내부 변수 ◀─────────────────────────
    private Vector2 _inputMove = Vector2.zero;
    private bool _inputRun = false;
    private bool _inputJump = false;
    private bool _inputAttack = false;
    private bool _inputGuard = false;

    private bool _isGrounded = true;
    private bool _isFalling = false;
    private bool _isHit = false;
    private bool _isDropImpact = false;
    private bool _isBound = false;

    private float _nextDropImpactTime = 0f;
    #endregion

    #region ─────────────────────────▶ 내부 메서드 ◀─────────────────────────
    // 바인딩 메서드
    private void TryBindInput()
    {
        // 바인딩 여부 & 키 매니저가 준비되었는지 확실히 검사
        var im = InputManager.Ins;
        if (_isBound || im == null || !im.IsReady)
        {
            return;
        }
        // 구독
        im.OnMove += HandleMove;
        im.OnRun += HandleRun;
        im.OnJump += HandleJump;
        im.OnAttack += HandleAttack;
        im.OnGuard += HandleGuard;
        _isBound = true;
        De.Log(_log, "플레이어 컨트롤러에서 바인드를 완료했습니다.");
    }
    private void TryUnBindInput()
    {
        // 바인딩 여부 & 키 매니저가 준비되었는지 확실히 검사
        var im = InputManager.Ins;
        if (!_isBound || im == null || !im.IsReady)
        {
            return;
        }
        // 구독
        im.OnMove -= HandleMove;
        im.OnRun -= HandleRun;
        im.OnJump -= HandleJump;
        im.OnAttack -= HandleAttack;
        im.OnGuard -= HandleGuard;
        _isBound = false;
        De.Log(_log, "플레이어 컨트롤러에서 언바인드를 완료했습니다.");
    }
    private void HandleMove(Vector2 v) => _inputMove = v;
    // True → KeyDown / False → KeyUp
    private void HandleRun(bool pressed) => _inputRun = pressed;
    private void HandleAttack(bool pressed) => _inputAttack = pressed;
    private void HandleGuard(bool pressed) => _inputGuard = pressed;
    private void HandleJump(bool pressed) => _inputJump = pressed;

    // 편의성 메서드
    private bool RefAutoConnect()
    {
        bool success = true;
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
            De.Log(_log, "플레이어 애니메이터 자동 연결을 시도했습니다.");
            if (De.IsNull(_animator))
            {
                success = false;
            }
        }
        if (_rb == null)
        {
            _rb = GetComponent<Rigidbody>();
            De.Log(_log, "플레이어 리지드바디 자동 연결을 시도했습니다.");
            if (De.IsNull(_rb))
            {
                success = false;
            }
        }
        if (_tr == null)
        {
            _tr = GetComponent<Transform>();
            De.Log(_log, "플레이어 트랜스폼 자동 연결을 시도했습니다.");
            if (De.IsNull(_tr))
            {
                success = false;
            }
        }
        if (_cameraTr == null)
        {
            if (!De.IsNull(Camera.main))
            {
                _cameraTr = Camera.main.transform;
                De.Log(_log, "플레이어 카메라를 자동 연결했습니다.");
            }
            else
            {
                success = false;
                De.IsNull(_cameraTr);
            }
        }
        if (_heart == null)
        {
            _heart = GetComponent<EntityHeart>();
            De.Log(_log, "피격 클래스 자동 연결을 시도했습니다.");
            if (De.IsNull(_heart))
            {
                success = false;
            }
        }
        return success;
    }

    // 데이터 빌드
    private void TickGroundCheck()
    {
        Vector3 startPos = transform.position + Vector3.up * _groundUpStick; // 시작점
        _isGrounded = Physics.Raycast(startPos, Vector3.down, _groundDownStick, _groundLayer); // 쏘아라
        De.DownRay(_ray, startPos, _groundDownStick, _isGrounded ? Color.green : Color.red, 0f);
    }
    
    private void TickDropImpactCheck() // Ground와 Falling 사이에 호출할 것
    {
        _isDropImpact = false;
        if (!_isGrounded)
        {
            return;
        }
        if (_isFalling)
        {
            if (Time.fixedTime < _nextDropImpactTime)
            {
                return;
            }
            if (_rb.velocity.y <= _dropImpactCondVelocity)
            {
                _nextDropImpactTime = Time.fixedTime + _dropImpactCooldown;
                _isDropImpact = true;
                De.Log(_log, "낙하 판정이 발생했습니다.");
            }
        }
    }
    private void TickFallingCheck()
    {
        if (!_isGrounded)
        {
            _isFalling = _rb.velocity.y < 0f;
        }
        else
        {
            _isFalling = false;
        }
    }
    private void TickHitCheck()
    {
        _isHit = _heart.IsHit();
    }
    private PlayerContext BuildContext()
    {
        return new PlayerContext
            (_rb, _animator, _cameraTr, _tr,
            _inputMove, _inputRun, _inputJump, _inputAttack, _inputGuard,
            _isGrounded, _isFalling, _isHit, _isDropImpact);
    }
    #endregion

    #region ─────────────────────────▶ 메시지 함수 ◀─────────────────────────
    private void Awake()
    {
        // 참조 연결
        if (!RefAutoConnect())
        {
            De.Print("필수 참조 연결 실패...", LogType.Assert);
            enabled = false;
            return;
        }
    }
    // 바인드
    private void OnEnable()
    {
        TryBindInput();
    }
    private void OnDisable()
    {
        TryUnBindInput();
    }
    private void OnDestroy()
    {
        TryUnBindInput();
    }
    // 상태 빌드 및 전달 & 호출
    private void FixedUpdate()
    {
        TickGroundCheck();
        TickDropImpactCheck();
        TickFallingCheck();
        TickHitCheck();
        PlayerContext context = BuildContext();
        _fsm.UpdateState(in context);
    }

    private void OnGUI()
    {
        if (_log)
        {
            PlayerContext context = BuildContext();
            _fsm.DrawText(in context);
        }
    }
    #endregion
}
