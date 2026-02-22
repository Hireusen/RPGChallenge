using static Constant;
using UnityEngine;

/// <summary>
/// 플레이어 오브젝트에 부착하는 C# 스크립트입니다.
/// 이동 키를 자동으로 입력받아 플레이어를 이동시킵니다.
/// </summary>
public class PlayerMover : MonoBehaviour
{
    #region ─────────────────────────▶ 인스펙터 ◀─────────────────────────
    [Header("필수 요소 등록")]
    [SerializeField] private Transform _player;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Transform _camera;
    [SerializeField] private Animator _animator;
    [SerializeField] private CharacterController _controller;

    [Header("이동 설정")]
    [SerializeField] private float _walkSpeed = 5f;
    [SerializeField] private float _runSpeed = 8f;
    [SerializeField] private float _rotateSharpness = 15f;
    [SerializeField] private float _jumpHeight = 1.2f;

    [Header("캐릭터 설정")]
    [SerializeField] private float _groundRadius = 0.2f;
    [SerializeField] private float _groundStick = 0.1f;

    [Header("파라미터")]
    [SerializeField] private string _paramSpeed = "aSpeed";
    [SerializeField] private string _paramRun = "bRun";
    [SerializeField] private string _paramJump = "tJump";
    #endregion

    #region ─────────────────────────▶ 내부 변수 ◀─────────────────────────
    private int _hashSpeed;
    private int _hashRun;
    private int _hashJump;
    #endregion

    #region ─────────────────────────▶ 내부 메서드 ◀─────────────────────────
    // 키를 입력받아 알맞은 메서드를 실행합니다.
    private void InputBranch()
    {
        // 이동
        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");
        if (v != 0f || h != 0f) {
            bool run;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
                run = true;
            } else {
                run = false;
            }
            GetMovement(v, h, run);
        }
        // 점프
        if (Input.GetKeyDown(KeyCode.Space)) {

        }
        // 방어
        if (Input.GetKeyDown(KeyCode.Mouse1)) {

        }
        // 공격
        else if (Input.GetKeyDown(KeyCode.Mouse0)) {

        }
        // 모션 1
        else if (Input.GetKeyDown(KeyCode.Alpha1)) {

        }
        // 모션 2
        else if (Input.GetKeyDown(KeyCode.Alpha2)) {

        }
        // 모션 3
        else if (Input.GetKeyDown(KeyCode.Alpha3)) {

        }
    }

    private bool IsGrounded()
    {
        return Physics.SphereCast(_player.position, _groundRadius, Vector3.down, out _, _groundStick, LAYER_TERRAINS);
    }

    // 4방향 이동
    private Vector3 GetMovement(float v, float h, bool run)
    {
        // 방향 벡터
        Vector3 movement = new Vector3(h, 0, v);
        movement = Vector3.ClampMagnitude(movement, 1f); // 이동 거리 제한
        // 이동량
        if (run) {
            movement *= _runSpeed;
        } else {
            movement *= _walkSpeed;
        }
        return movement;
    }

    // 현재 점프 
    private void TryJump()
    {
        if (!IsGrounded())
            return;
        
    }

    // 인스펙터 참조 자동 연결
    private void AutoInspector()
    {
        if (_player == null) {
            _player = transform;
        }
        if (_rb == null) {
            TryGetComponent<Rigidbody>(out _rb);
        }
        if (_camera == null) {
            var camera = Camera.main;
            if (camera != null) {
                De.Print("메인 카메라가 존재하지 않습니다!", LogType.Assert);
            } else {
                _camera = camera.transform;
            }
        }
        if (_animator == null) {
            TryGetComponent<Animator>(out _animator);
        }
        if (_controller == null) {
            TryGetComponent<CharacterController>(out _controller);
        }
    }

    // 인스펙터 유효성 검사
    private bool HasEssentials()
    {
        if (De.IsNull(_player)
            || De.IsNull(_rb)
            || De.IsNull(_camera)
            || De.IsNull(_animator)
            || De.IsNull(_controller)
            || De.IsTrue(string.IsNullOrEmpty(_paramSpeed))
            || De.IsTrue(string.IsNullOrEmpty(_paramRun))
            || De.IsTrue(string.IsNullOrEmpty(_paramJump))
            ) {
            enabled = false;
            return false;
        }
        De.Print("PlayerMover 유효성 검사를 통과했습니다.", LogType.Log);
        return true;
    }
    #endregion

    #region ─────────────────────────▶ 메시지 함수 ◀─────────────────────────
    private void Awake()
    {
        AutoInspector();
    }

    private void OnEnable()
    {
        if (HasEssentials()) {
            _hashSpeed = Animator.StringToHash(_paramSpeed);
            _hashRun = Animator.StringToHash(_paramRun);
            _hashJump = Animator.StringToHash(_paramJump);
            De.Print($"해시를 갱신했습니다. ({_hashSpeed}, {_hashRun}, {_hashJump})");
        }
    }

    private void Update()
    {
        if (_player == null)
            return;

        InputBranch();
    }

    private void Reset()
    {
        AutoInspector();
    }
    #endregion
}
