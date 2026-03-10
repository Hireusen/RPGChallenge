using UnityEngine;

[System.Serializable]
public class PlayerIdleState : IPlayerState
{
    [Header("파라미터")]
    [SerializeField] private string _idleParam = "fIdle";
    [SerializeField, Range(0f, 1f)] private float _dampTime = 0.12f;

    [Header("디버그")]
    [SerializeField] private bool _log = false;

    private int _hashIdle;
    private bool _hasIdleParam;

    #region ─────────────────────────▶ 공개 멤버 ◀─────────────────────────
    public PlayerIdleState()
    {
        De.Log(_log, "플레이어 Idle 생성자가 호출되었습니다.");
        _hasIdleParam = !string.IsNullOrEmpty(_idleParam);
        if (_hasIdleParam)
        {
            _hashIdle = Animator.StringToHash(_idleParam);
            De.Log(_log, "성공적으로 Idle Hash를 생성했습니다.");
        }
    }

    public void Enter(in PlayerContext context, ref float stateChangeLockTime)
    {
        context.rb.velocity = Vector3.zero;
        context.animator.SetFloat(_hashIdle, 0.9f);
        context.animator.SetFloat(_hashIdle, 1f, _dampTime, Time.fixedDeltaTime);
    }

    public void Frame(in PlayerContext context)
    {
        context.animator.SetFloat(_hashIdle, 1f, _dampTime, Time.fixedDeltaTime);
    }

    public void Exit(in PlayerContext context) {
        context.animator.SetFloat(_hashIdle, 0.89f);
        context.animator.SetFloat(_hashIdle, 0f);
    }
    #endregion
}
