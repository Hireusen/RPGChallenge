using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// ~ 오브젝트에 부착하는 C# 스크립트입니다.
/// Input에 대한 디스패처를 수행합니다.
/// </summary>
public class InputDisfactcher : MonoBehaviour
{
    // 들어오는 입력을 이벤트로 방송하는 역할을 수행하는 클래스입니다.
    // 누가 움직이는지 모르고 알 필요도 없습니다.
    // 입력을 읽고 이벤트로 뿌립니다.
    #region ─────────────────────────▶ 인스펙터 ◀─────────────────────────
    [Header("필수 요소 등록")]
    [SerializeField] private InputActionReference _move;
    [SerializeField] private InputActionReference _jump;
    [SerializeField] private InputActionReference _pause;

    [Header("사용자 정의 설정")]
    [SerializeField] private bool _log = false;
    #endregion

    #region ─────────────────────────▶ 내부 변수 ◀─────────────────────────
    public static InputDisfactcher Ins { get; set; }
    public event System.Action<Vector2> OnMove;
    public event System.Action OnJump;
    public event System.Action OnPause;

    private bool _isReady = false; // 중복 입력 방지 (바인딩 / 구독을 한 번만 하게 막는 것)
    #endregion

    #region ─────────────────────────▶ 내부 메서드 ◀─────────────────────────
    private void TryBind()
    {
        // 콜백은 여러번 호출되면 호출된만큼 그대로 수행하기 때문에 필수 작업이다.
        if (_isReady) {
            return;
        }
        _isReady = true;
        // 액션이 비어있다는 건 세팅을 안했거나 잘못했거나
        if(_move == null || _move.action == null) {
            return;
        }
        if(_jump == null || _jump.action == null) {
            return;
        }
        if(_pause == null || _pause.action == null) {
            return;
        }
        // performed =
        // 콜백을 매개변수로 받는 느낌을 잡아줘야 한다.
        // 뉴 인풋을 사용할 때 value 타입을 사용하면 연결과 취소가 한 세트다.
        // 키를 누르고 있는 동안 값이 변화되거나 유지되고 있으면 들어온다.
        _move.action.performed += OnMovePerformed; // WASD / 2개 키를 조합하거나 할때 들어온다는 얘기
        _move.action.canceled += OnMoveCanceled; // 키를 떼면 들어오는 것. 이걸 안 넣으면 키를 떼도 value값이 유지되어버린다. (계속 움직임)
        _jump.action.performed += OnJumpPerformed;
        _pause.action.performed += OnPausePerformed;
        // 만약 유지나 해제가 필요하다면 started / canceled 사용하는 식으로 확장할 수 있다.
        // 단발성으로 날릴거면 performed 쓴다.
        _isReady = true;
        // 로그 무조건 찍어보는게 좋다.
        // 흐름에 따라서 안 들어가는 경우가 되게 많다.
        if (_log) {
            De.Print($"바인드 완료 (Ready = {_isReady})");
        }
    }

    private void UnBind()
    {
        if (!_isReady) {
            return;
        }
        // 이렇게 해제 작업 안하면 중복 바인딩이 들어갈 수 있다.
        // 호출되는 횟수에 따라 누적되어버린다.
        if(_move != null && _move.action != null) {
            _move.action.performed -= OnMovePerformed;
            _move.action.canceled -= OnMoveCanceled;
        }
        if(_jump != null && _jump.action != null) {
            _jump.action.performed -= OnJumpPerformed;
        }
        if(_pause != null && _pause.action != null) {
            _pause.action.performed -= OnPausePerformed;
        }
        _isReady = false;
        De.Print("언바인드 완료!");
    }

    private void EnableActions(bool enable)
    {
        if (!_isReady) {
            return;
        }
        // 입력을 받겠다.
        if (enable) {
            _move.action.Enable();
            _jump.action.Enable();
            _pause.action.Enable();
        }
        // 입력을 받지 않겠다.
        else {
            _move.action.Disable();
            _jump.action.Disable();
            _pause.action.Disable();
        }
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        // 합성을 해준다. string, int 등 이 값들을 에셋으로 설정해 놓은 타입에 맞게 합성을 진행한다.
        // W를 누르면 (0, 1), A를 누르면 (-1, 0), W + D (1, 1) 이런 느낌으로 진행된다.
        Vector2 v = ctx.ReadValue<Vector2>();
        if (_log) { // 로그 찍는 게 추적이 매우 용이하다.
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

    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        if (_log) {
            De.Print($"On Jump Performed");
        }
        OnJump?.Invoke();
    }

    private void OnPausePerformed(InputAction.CallbackContext ctx)
    {
        if (_log) {
            De.Print($"On Pause Performed");
        }
        OnPause?.Invoke();
    }
    #endregion

    #region ─────────────────────────▶ 메시지 함수 ◀─────────────────────────
    private void Awake()
    {
        if (Ins != null && Ins != this) {
            Destroy(gameObject);
            return;
        }
        Ins = this;
        // 바인딩 처리
        TryBind();
    }

    private void OnEnable()
    {
        TryBind();
        EnableActions(true);
    }

    private void OnDisable()
    {
        EnableActions(false);
    }

    private void OnDestroy()
    {
        if(Ins == this) {
            Ins = null;
            // 바인드 해제
            UnBind();
        }
    }
    #endregion
}
