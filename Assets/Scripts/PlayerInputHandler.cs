using UnityEngine;
using System.Collections;

/// <summary>
/// 플레이어 오브젝트에 부착하는 C# 스크립트입니다.
/// 뉴 인풋 시스템을 통한 간단한 조작을 지원합니다.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerInputHandler : MonoBehaviour
{
    #region ─────────────────────────▶ 인스펙터 ◀─────────────────────────
    [Header("필수 요소 등록")]
    [SerializeField] private Transform _cameraTr;

    [Header("사용자 정의 설정")]
    [SerializeField] private float _moveSpeed = 6f;
    [SerializeField] private float _jumpHeight = 1.2f;
    [SerializeField] private float _rotateSharpness = 15f;
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private float _groundStick = -2f;
    [SerializeField] private bool _log = false;
    #endregion

    #region ─────────────────────────▶ 내부 변수 ◀─────────────────────────
    private CharacterController _cc;
    // 디스패처가 쏘는 Move(Vector2)는 순간이기 때문에 지속성을 가지기 위해서는 저장해두는게 훨씬 안정적이다.
    private Vector2 _moveInput;
    private bool _jumpRequested;
    // 리지드바디를 사용 안하고 있으니 직접 y속도를 누적시키기 위한 변수
    private bool _verticalVel;
    private Coroutine _bindCo;
    #endregion

    #region ─────────────────────────▶ 내부 메서드 ◀─────────────────────────
    private void HandleMove(Vector2 v)
    {
        _moveInput = Vector2.ClampMagnitude(v, 1f);
        float delta = _moveSpeed * Time.deltaTime;

    }
    private void HandleJump()
    {
        
    }
    private void HandlePause()
    {
        
    }

    private void SubscribeInput()
    {
        var input = InputDisfactcher.Ins;
        if (De.IsNull(input)) {
            return;
        }
        input.OnMove -= HandleMove;
        input.OnJump += HandleJump;
        input.OnPause += HandlePause;
        De.Print("플레이어 이벤트 등록을 완료했습니다.");
    }

    private void UnSubscribeInput()
    {
        var input = InputDisfactcher.Ins;
        if (De.IsNull(input)) {
            return;
        }
        input.OnMove -= HandleMove;
        input.OnJump -= HandleJump;
        input.OnPause -= HandlePause;
        De.Print("플레이어 이벤트 등록을 해제했습니다.");
    }

    private IEnumerator CoBindDispatcher()
    {
        while (InputDisfactcher.Ins == null) {
            yield return null;
        }
        SubscribeInput();
    }
    #endregion

    #region ─────────────────────────▶ 메시지 함수 ◀─────────────────────────
    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        if(_cameraTr == null && Camera.main != null) {
            _cameraTr = Camera.main.transform;
        }
    }

    private void OnEnable()
    {
        // 방어 코드는 옳다
        // 여기에 담긴 디테일을 아시겠어요? 혹시 남아있는 코루틴이 있다면..
        if(_bindCo != null) {
            StopCoroutine(_bindCo);
        }
        // 구독
        _bindCo = StartCoroutine(CoBindDispatcher());
    }

    private void OnDisable()
    {
        if (_bindCo != null) {
            StopCoroutine(_bindCo);
        }
        _bindCo = null;
        // 구독 해제
        var input = InputDisfactcher.Ins;
        if (input != null) {
            UnSubscribeInput();
        }
    }
    #endregion
}
