using UnityEngine;

[System.Serializable]
public class PlayerWalkState : IPlayerState
{
    [Header("파라미터")]
    [SerializeField] private string _walkParam = "fWalk";
    [SerializeField, Range(0f, 1f)] private float _dampTime = 0.12f;

    [Header("이동 설정")]
    [SerializeField] private float _walkSpeed = 3f;
    [SerializeField] private float _rotateSharpness = 15f;

    [Header("디버그")]
    [SerializeField] private bool _log = false;

    private int _hashWalk;
    private bool _hasWalkParam;

    #region ─────────────────────────▶ 공개 멤버 ◀─────────────────────────
    public PlayerWalkState()
    {
        De.Log(_log, "플레이어 Walk 생성자가 호출되었습니다.");
        _hasWalkParam = !string.IsNullOrEmpty(_walkParam);
        if (_hasWalkParam)
        {
            _hashWalk = Animator.StringToHash(_walkParam);
            De.Log(_log, "성공적으로 Walk Hash를 생성했습니다.");
        }
    }

    public void Enter(in PlayerContext context, ref float stateChangeLockTime)
    {
        context.animator.SetFloat(_hashWalk, 0.9f);
        context.animator.SetFloat(_hashWalk, 1f, _dampTime, Time.fixedDeltaTime);
    }

    public void Frame(in PlayerContext context)
    {
        context.animator.SetFloat(_hashWalk, 1f, _dampTime, Time.fixedDeltaTime);

        Vector3 dir = UPlayerState.BuildMoveDirection(in context);
        if (dir != Vector3.zero)
        {
            UPlayerState.SetGroundVelocity(in context, dir, _walkSpeed);
            UPlayerState.Rotate(in context, dir, _rotateSharpness);
        }
        else
        {
            UPlayerState.StopGroundVelocity(in context);
        }
    }

    public void Exit(in PlayerContext context)
    {
        context.animator.SetFloat(_hashWalk, 0.89f);
        context.animator.SetFloat(_hashWalk, 0f, _dampTime, Time.fixedDeltaTime);
    }
    #endregion
}
