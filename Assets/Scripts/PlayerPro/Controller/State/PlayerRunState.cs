using UnityEngine;

[System.Serializable]
public class PlayerRunState : IPlayerState
{
    [Header("파라미터")]
    [SerializeField] private string _runParam = "fRun";
    [SerializeField, Range(0f, 1f)] private float _dampTime = 0.12f;

    [Header("이동 설정")]
    [SerializeField] private float _runSpeed = 9f;
    [SerializeField] private float _rotateSharpness = 3f;

    [Header("디버그")]
    [SerializeField] private bool _log = false;

    private int _hashRun;
    private bool _hasRunParam;

    #region ─────────────────────────▶ 공개 멤버 ◀─────────────────────────
    public PlayerRunState()
    {
        De.Log(_log, "플레이어 Run 생성자가 호출되었습니다.");
        _hasRunParam = !string.IsNullOrEmpty(_runParam);
        if (_hasRunParam)
        {
            _hashRun = Animator.StringToHash(_runParam);
            De.Log(_log, "성공적으로 Run Hash를 생성했습니다.");
        }
    }

    public void Enter(in PlayerContext context, ref float stateChangeLockTime)
    {
        context.animator.SetFloat(_hashRun, 0.9f);
        context.animator.SetFloat(_hashRun, 1f, _dampTime, Time.fixedDeltaTime);
    }

    public void Frame(in PlayerContext context)
    {
        context.animator.SetFloat(_hashRun, 1f, _dampTime, Time.fixedDeltaTime);
        // 이동 및 회전
        Vector3 dir = UPlayerState.BuildMoveDirection(in context);
        if (dir != Vector3.zero)
        {
            UPlayerState.SetGroundVelocity(in context, dir, _runSpeed);
            UPlayerState.Rotate(in context, dir, _rotateSharpness);
        }
    }

    public void Exit(in PlayerContext context)
    {
        context.animator.SetFloat(_hashRun, 0.89f);
        context.animator.SetFloat(_hashRun, 0f, _dampTime, Time.fixedDeltaTime);
    }
    #endregion
}
