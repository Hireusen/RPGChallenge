using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 빈 오브젝트에 부착하는 C# 스크립트입니다.
/// Input System으로 키 입력을 받아 이벤트를 뿌립니다.
/// </summary>
public class InputManager : MonoBehaviour
{
    #region ─────────────────────────▶ 인스펙터 ◀─────────────────────────
    [Header("필수 요소 등록")]
    [SerializeField] private InputActionReference _move;
    [SerializeField] private InputActionReference _run;
    [SerializeField] private InputActionReference _jump;
    [SerializeField] private InputActionReference _attack;
    [SerializeField] private InputActionReference _guard;

    [Header("사용자 정의 설정")]
    [SerializeField] private bool _log = false;
    #endregion

    #region ─────────────────────────▶ 내부 변수 ◀─────────────────────────
    public static InputManager Ins { get; private set; }

    // 싱글톤 생성 완료 시 발행
    public static event Action OnReady;

    public event Action<Vector2> OnMove;
    public event Action<bool> OnRun;
    public event Action<bool> OnJump;
    public event Action<bool> OnAttack;
    public event Action<bool> OnGuard;

    private bool _isReady = false;
    #endregion

    #region ─────────────────────────▶ 내부 메서드 ◀─────────────────────────
    // 편의성
    private bool ActionIsNull(InputActionReference actionRef)
    {
        if (actionRef == null) {
            De.Print($"인풋 매니저에서 액션 주소({actionRef})가 Null입니다!");
            return true;
        }
        if (actionRef.action == null) {
            De.Print($"인풋 매니저에서 액션({actionRef.action})이 Null입니다!");
            return true;
        }
        return false;
    }
    // 모든 Input Action에 가입
    private void RegisterActions()
    {
        _move.action.performed += OnMovePerformed;
        _move.action.canceled += OnMoveCanceled;
        _run.action.performed += OnRunPerformed;
        _run.action.canceled += OnRunCanceled;
        _jump.action.performed += OnJumpPerformed;
        _jump.action.canceled += OnJumpCanceled;
        _attack.action.performed += OnAttackPerformed;
        _attack.action.canceled += OnAttackCanceled;
        _guard.action.performed += OnGuardPerformed;
        _guard.action.canceled += OnGuardCanceled;
    }
    // 모든 Input Action에서 탈퇴
    private void UnRegisterActions()
    {
        if (_move != null && _move.action != null) {
            _move.action.performed -= OnMovePerformed;
            _move.action.canceled -= OnMoveCanceled;
        }
        if (_run != null && _run.action != null) {
            _run.action.performed -= OnRunPerformed;
            _run.action.canceled -= OnRunCanceled;
        }
        if (_jump != null && _jump.action != null) {
            _jump.action.performed -= OnJumpPerformed;
            _jump.action.canceled -= OnJumpCanceled;
        }
        if (_attack != null && _attack.action != null) {
            _attack.action.performed -= OnAttackPerformed;
            _attack.action.canceled -= OnAttackCanceled;
        }
        if (_guard != null && _guard.action != null) {
            _guard.action.performed -= OnGuardPerformed;
            _guard.action.canceled -= OnGuardCanceled;
        }
    }
    // 안전하게 가입
    private void TryBind()
    {
        if (_isReady) {
            return;
        }
        // 액션 세팅 여부 확인
        if (ActionIsNull(_move)
            || ActionIsNull(_run)
            || ActionIsNull(_jump)
            || ActionIsNull(_attack)
            || ActionIsNull(_guard)
        ) {
            return;
        }
        // performed + canceled 모두 등록
        RegisterActions();
        // 등록 완료
        _isReady = true;
        if (_log) {
            De.Print("Input Manager 바인드 완료 !");
        }
    }
    // 안전하게 탈퇴
    private void UnBind()
    {
        if (!_isReady) {
            return;
        }
        // 해제 작업 안하면 중복 바인딩이 누적된다
        UnRegisterActions();
        // 등록 해제
        _isReady = false;
        De.Print("Input Manager 언바인드 완료 !");
    }
    // Input Manager가 활성화 / 비활성화될 경우
    private void EnableActions(bool enable)
    {
        if (!_isReady) {
            return;
        }
        if (enable) {
            _move.action.Enable();
            _run.action.Enable();
            _jump.action.Enable();
            _attack.action.Enable();
            _guard.action.Enable();
        } else {
            _move.action.Disable();
            _run.action.Disable();
            _jump.action.Disable();
            _attack.action.Disable();
            _guard.action.Disable();
        }
    }
    #endregion

    #region ─────────────────────────▶ 콜백 함수 ◀─────────────────────────
    // 이동
    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        Vector2 v = ctx.ReadValue<Vector2>();
        if (_log) {
            De.Print($"On Move Performed = {v}");
        }
        OnMove?.Invoke(v);
    }
    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        if (_log) {
            De.Print($"On Move Canceled = {Vector2.zero}");
        }
        OnMove?.Invoke(Vector2.zero);
    }
    // 달리기
    private void OnRunPerformed(InputAction.CallbackContext ctx)
    {
        if (_log) {
            De.Print($"On Run Performed (true)");
        }
        OnRun?.Invoke(true);
    }
    private void OnRunCanceled(InputAction.CallbackContext ctx)
    {
        if (_log) {
            De.Print($"On Run Canceled (false)");
        }
        OnRun?.Invoke(false);
    }
    // 점프
    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        if (_log) {
            De.Print($"On Jump Performed (true)");
        }
        OnJump?.Invoke(true);
    }
    private void OnJumpCanceled(InputAction.CallbackContext ctx)
    {
        if (_log) {
            De.Print($"On Jump Canceled (false)");
        }
        OnJump?.Invoke(false);
    }
    // 공격
    private void OnAttackPerformed(InputAction.CallbackContext ctx)
    {
        if (_log) {
            De.Print($"On Attack Performed (true)");
        }
        OnAttack?.Invoke(true);
    }
    private void OnAttackCanceled(InputAction.CallbackContext ctx)
    {
        if (_log) {
            De.Print($"On Attack Canceled (false)");
        }
        OnAttack?.Invoke(false);
    }
    // 가드
    private void OnGuardPerformed(InputAction.CallbackContext ctx)
    {
        if (_log) {
            De.Print($"On Guard Performed (true)");
        }
        OnGuard?.Invoke(true);
    }
    private void OnGuardCanceled(InputAction.CallbackContext ctx)
    {
        if (_log) {
            De.Print($"On Guard Canceled (false)");
        }
        OnGuard?.Invoke(false);
    }
    #endregion

    #region ─────────────────────────▶ 메시지 함수 ◀─────────────────────────
    private void Awake()
    {
        // 싱글턴
        if (Ins != null && Ins != this) {
            Destroy(gameObject);
            return;
        }
        Ins = this;
        // Actions 가입 및 공지
        TryBind();
        OnReady?.Invoke();
        // 유니티를 껏다 키면 에러가 해결되는 걸 경험한 뒤로 느낌이 좋지 않았습니다.
        // 강사님의 방어 코드가 과하다고 생각한 적도 있는데 모노비헤이비어조차 신뢰할 수가 없군요.
    }

    private void OnEnable()
    {
        // Actions 가입 및 활성화
        TryBind();
        EnableActions(true);
    }

    private void OnDisable()
    {
        // Actions 비활성화
        EnableActions(false);
    }

    private void OnDestroy()
    {
        // 싱글턴
        if (Ins == this) {
            Ins = null;
            // Actions 탈퇴
            UnBind();
        }
    }
    #endregion
}
