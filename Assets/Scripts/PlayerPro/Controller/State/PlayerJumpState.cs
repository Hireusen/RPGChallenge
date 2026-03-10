using UnityEngine;

[System.Serializable]
public class PlayerJumpState : IPlayerState
{
    [Header("파라미터")]
    [SerializeField] private string _jumpParam = "fJump";
    [SerializeField, Range(0f, 1f)] private float _dampTime = 0.12f;

    [Header("점프 설정")]
    [SerializeField] private float _jumpForce = 7f;
    [SerializeField] private float _doubleJumpForce = 7f;
    [SerializeField] private float _jumpCooldown = 0.3f;

    [Header("이동 설정")]
    [SerializeField] private float _walkSpeed = 5.4f;
    [SerializeField] private float _runSpeed = 8.1f;
    [SerializeField] private float _rotateSharpness = 1.5f;

    [Header("디버그")]
    [SerializeField] private bool _log = false;

    private int _hashJump;
    private bool _hasJumpParam;
    private bool _canDoubleJump;
    private float _nextDoubleJumpTime = 0f;

    #region ─────────────────────────▶ 공개 멤버 ◀─────────────────────────
    public PlayerJumpState()
    {
        De.Log(_log, "플레이어 Jump 생성자가 호출되었습니다.");
        _hasJumpParam = !string.IsNullOrEmpty(_jumpParam);
        if (_hasJumpParam)
        {
            _hashJump = Animator.StringToHash(_jumpParam);
            De.Log(_log, "성공적으로 Jump Hash를 생성했습니다.");
        }
    }

    public void Enter(in PlayerContext context, ref float stateChangeLockTime)
    {
        _canDoubleJump = true;
        Jump(in context, _jumpForce);
        context.animator.SetFloat(_hashJump, 0.9f);
        context.animator.SetFloat(_hashJump, 1f, _dampTime, Time.fixedDeltaTime);
    }

    public void Frame(in PlayerContext context)
    {
        context.animator.SetFloat(_hashJump, 1f, _dampTime, Time.fixedDeltaTime);
        // 공중 이동
        Vector3 dir = UPlayerState.BuildMoveDirection(in context);
        if (dir != Vector3.zero)
        {
            float speed = context.inputRun ? _runSpeed : _walkSpeed;
            UPlayerState.SetGroundVelocity(in context, dir, speed);
            UPlayerState.Rotate(in context, dir, _rotateSharpness);
        }
        // 더블 점프
        if (context.inputJump && _canDoubleJump && Time.fixedTime >= _nextDoubleJumpTime)
        {
            _canDoubleJump = false;
            Jump(in context, _doubleJumpForce);
            De.Log(_log, $"더블 점프 실행!");
        }
        De.Log(_log, $"{Time.fixedTime} >= {_nextDoubleJumpTime}");
    }

    public void Exit(in PlayerContext context)
    {
        context.animator.SetFloat(_hashJump, 0.89f);
        context.animator.SetFloat(_hashJump, 0f, _dampTime, Time.fixedDeltaTime);
    }
    #endregion

    #region ─────────────────────────▶ 내부 메서드 ◀─────────────────────────
    private void Jump(in PlayerContext context, float force)
    {
        context.rb.velocity = new Vector3(context.rb.velocity.x, 0f, context.rb.velocity.z);
        context.rb.AddForce(Vector3.up * force, ForceMode.Impulse);
        _nextDoubleJumpTime = Time.fixedTime + _jumpCooldown;
    }
    #endregion
}
