using UnityEngine;

[System.Serializable]
public class PlayerGuardState : IPlayerState
{
    [Header("파라미터")]
    [SerializeField] private string _guardParam = "fGuard";
    [SerializeField, Range(0f, 1f)] private float _dampTime = 0.12f;

    [Header("회전 설정")]
    [SerializeField] private float _rotateSharpness = 1f;

    [Header("디버그")]
    [SerializeField] private bool _log = false;

    private int _hashGuard;
    private bool _hasGuardParam;

    #region ─────────────────────────▶ 공개 멤버 ◀─────────────────────────
    public PlayerGuardState()
    {
        De.Log(_log, "플레이어 Guard 생성자가 호출되었습니다.");
        _hasGuardParam = !string.IsNullOrEmpty(_guardParam);
        if (_hasGuardParam)
        {
            _hashGuard = Animator.StringToHash(_guardParam);
            De.Log(_log, "성공적으로 Guard Hash를 생성했습니다.");
        }
    }

    public void Enter(in PlayerContext context, ref float stateChangeLockTime)
    {
        context.rb.velocity = Vector3.zero;
        context.animator.SetFloat(_hashGuard, 0.9f);
        context.animator.SetFloat(_hashGuard, 1f, _dampTime, Time.fixedDeltaTime);
    }

    public void Frame(in PlayerContext context)
    {
        context.animator.SetFloat(_hashGuard, 1f, _dampTime, Time.fixedDeltaTime);
        // 회전
        Vector3 dir = UPlayerState.BuildMoveDirection(in context);
        if (dir != Vector3.zero)
        {
            UPlayerState.Rotate(in context, dir, _rotateSharpness);
        }
    }

    public void Exit(in PlayerContext context)
    {
        context.animator.SetFloat(_hashGuard, 0.89f);
        context.animator.SetFloat(_hashGuard, 0f, _dampTime, Time.fixedDeltaTime);
    }
    #endregion
}
