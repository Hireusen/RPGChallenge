using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

// 참고 : CC는 Character Control다.
public class CMecanimController : MonoBehaviour
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 인스펙터 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    [Header("참조하기")]
    [SerializeField] private Animator _animator;
    [SerializeField] private CharacterController _controller;
    [Header("카메라 기준 이동")]
    [SerializeField] private Transform _cameraTsf;
    [Header("이동 설정")]
    [SerializeField] private float _walkSpeed = 5f;
    [SerializeField] private float _runSpeed = 8f;
    [SerializeField] private float _rotateSharpness = 15f;
    [Header("점프 설정")]
    [SerializeField] private float _jumpHeight = 1.2f;
    [SerializeField] private float _gravity = -9.81f; // 중력 가속도 근사값
    [SerializeField] private float _groundStick = -2f; // 안정적으로 지상에 True를 걸 수 있도록 하는 트릭 (경사 등에서 유효)
    [Header("애니메이터 파라미터")]
    [SerializeField] private string _paramSpeed = "aSpeed"; // float
    [SerializeField] private string _paramRun = "bRun"; // bool
    [SerializeField] private string _paramJump = "tJump"; // trigger
    [Header("애니메이터 튜닝")]
    // aSpeed를 보간하여  딱딱 끊어지는 느낌의 조작감을 부드러운 느낌으로 개선하겠다.
    [SerializeField] private float _speedDamp = 0.12f;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 변수 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    // 수직 속도
    private float _verticalVel;
    // 파라미터 해시 (문자열로 접근하기 때문에 Hash로 만들어서 빠르고 안전하게 사용하겠다는 의도)
    private int _hashSpeed;
    private int _hashRun;
    private int _hashJump;
    private bool _hasRunParam;
    private bool _hasJumpParam;
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    private Vector3 BuildMoveDirection(Vector3 input)
    {
        if (_cameraTsf == null) {
            return input.normalized;
        }
        Vector3 camForward = Vector3.ProjectOnPlane(_cameraTsf.forward, Vector3.up).normalized;
        Vector3 camRight = Vector3.ProjectOnPlane(_cameraTsf.right, Vector3.up).normalized;
        // 카메라의 F, R을 구해서 바닥 평면에 투영시키겠다.
        // 카메라가 기울어져서 y성분이 섞여서 유저가 보는 화면이 기울어지지 않게 하기 위함
        Vector3 dir = camForward * input.z + camRight * input.x;
        return dir.normalized; // 최종 이동 방향
    }
    private bool TickJumpAndGravity(bool jumpKeyDown)
    {
        bool jumped = false;
        // isGrounded 컨트롤러가 바닥에 닿아있다고 판단하는 상태
        // 바닥 경사, 턱, 틈에서 잘못된 판단을 내릴수도 있다.
        // 바닥과 바닥 사이에 틈이 있는 경우가 제일 문제다.
        // 틈이 발견되면 디자이너에게 맡기거나 트릭으로 어떻게든 한다. 지형으로 가려놓던지.. 아예 가지 말던지..
        if (_controller.isGrounded) {
            // 바닥에 붙어있으면, y속도가 음수이면, 너무 떨어지지 않도록 고정한다.
            if (_verticalVel < 0f) {
                _verticalVel = _groundStick; // 미세한 틈, 경사를 안전하게 처리해준다.
            }
            if (jumpKeyDown) {
                // 점프 → 원하는 높이(h)에서 속도가 0이 되도록 시작 속도(v)를 역으로 계산한다.
                // 점프쓸 때 쓰는 공식 : v = sqrt(h * -2g)
                // 등가속도 운동 : _verticalVel += g * dt (중력이 적용된다)
                _verticalVel = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
                jumped = true;
            }
        }

        _verticalVel += _gravity * Time.deltaTime;
        return jumped;
    }
    private void TickRotate(Vector3 moveDir)
    {
        // 이동중일 때만 회전합니다.
        if (moveDir.sqrMagnitude < 0.0001f)
            return;
        Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
        float t = 1f - Mathf.Exp(-_rotateSharpness * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, t);
    }
    private bool HasEssentials()
    {
        if (De.IsNull(_controller)) {
            enabled = false;
            return false;
        }
        if (De.IsNull(_animator)) {
            enabled = false;
            return false;
        }
        return true;
    }
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 메시지 함수 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    private void Awake()
    {
        if (_controller == null)
            _controller = GetComponent<CharacterController>();
        if (_animator == null)
            _animator = GetComponent<Animator>();
        if (!HasEssentials())
            return;
        if (_cameraTsf == null && Camera.main != null) {
            _cameraTsf = Camera.main.transform;
        }
        // 해시 준비
        _hashSpeed = Animator.StringToHash(_paramSpeed);
        // 캐싱할 때 파라미터가 비어있으면 사용하지 않겠다.
        _hasRunParam = !string.IsNullOrEmpty(_paramRun);
        if (_hasRunParam)
            _hashRun = Animator.StringToHash(_paramRun);
        _hasJumpParam = !string.IsNullOrEmpty(_paramJump);
        if (_hasJumpParam)
            _hashJump = Animator.StringToHash(_paramJump);
    }
    private void Start()
    {
        print("컨트롤러 세팅이 완료되었습니다.");
    }
    // 컨트롤러의 특성상 업데이트 의존도가 높다.
    // 1. 입력 받기 / 이동 방향 계산 / 이동 속도 계산 / 점프와 중력 / CC에 반영 .Move / 애니메이터 파라미터 업데이트
    private void Update()
    {
        if (!HasEssentials())
            return;
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 input = new Vector3(h, 0, v);

        // 벡터의 크기를 제한하는 클램프 함수
        // 대각선 이동이 더 빠르지 않도록 범위를 제한한다.
        input = Vector3.ClampMagnitude(input, 1f);

        // 이동 방향 (입력이 거의 없다면 zero로 처리)
        Vector3 moveDir = input.sqrMagnitude > 0.0001f ? BuildMoveDirection(input) : Vector3.zero;

        // 속도 계산
        bool isRunKey = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float speed = (isRunKey ? _runSpeed : _walkSpeed);

        // 점프
        /*  이런 방식은 isGrounded의 판정 타이밍때문에 점프가 걸리지 않는 경우가 있다.
            if (Input.GetKeyDown(KeyCode.Space) && _controller.isGrounded){

            }
        */
        bool jumpKeyDown = Input.GetKeyDown(KeyCode.Space);
        bool jumpedThisFrame = TickJumpAndGravity(jumpKeyDown);
        if(_hasJumpParam && jumpedThisFrame) {
            _animator.SetTrigger(_hashJump);
        }

        // 이동 - 수평 이동과 수직 속도를 합쳐서 이동한다.
        Vector3 velocity = moveDir * speed;
        velocity.y = _verticalVel;
        _controller.Move(velocity * Time.deltaTime);
        TickRotate(moveDir);

        float speed01 = moveDir.magnitude * (isRunKey ? 1f : 0.5f);
        // aSpeed는 0~1범위로 사용하면 전환 조건 유지가 쉽다.
        // 즉 정규화된 값이 관리가 쉽다.
        _animator.SetFloat(_hashSpeed, speed01, _speedDamp, Time.deltaTime);
        if (_hasRunParam) {
            _animator.SetBool(_hashRun, isRunKey && moveDir.sqrMagnitude > 0.0001f);
        }

        if (Input.GetKeyDown(KeyCode.Z)) {
            _animator.SetTrigger("tAttack");
        }
        if (Input.GetKeyDown(KeyCode.P)) {
            _animator.SetTrigger("tWin");
        }
    }
    #endregion
}
