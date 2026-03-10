using UnityEngine;

/// <summary>
/// 클래스의 설계 의도입니다.
/// </summary>
[System.Serializable]
public class PlayerHitState : IPlayerState
{
    [Header("파라미터")]
    [SerializeField] private string _hitParam = "fHit";
    [SerializeField, Range(0f, 1f)] private float _dampTime = 0.5f;
    [SerializeField, Range(0f, 2f)] private float _lockTime = 0.3f;

    [Header("디버그")]
    [SerializeField] private bool _log = false;

    private int _hashHit;
    private bool _hasHitParam;

    #region ─────────────────────────▶ 공개 멤버 ◀─────────────────────────
    public PlayerHitState()
    {
        De.Log(_log, "플레이어 Hit 생성자가 호출되었습니다.");
        _hasHitParam = !string.IsNullOrEmpty(_hitParam);
        if (_hasHitParam)
        {
            _hashHit = Animator.StringToHash(_hitParam);
            De.Log(_log, "성공적으로 Hit을 생성했습니다.");
        }
    }

    public void Enter(in PlayerContext context, ref float stateChangeLockTime)
    {
        stateChangeLockTime = Time.fixedTime + _lockTime;
        context.rb.velocity = Vector3.zero;
        context.animator.SetFloat(_hashHit, 0.9f);
        context.animator.SetFloat(_hashHit, 1f, _dampTime, Time.fixedDeltaTime);
    }

    public void Frame(in PlayerContext context)
    {
        context.animator.SetFloat(_hashHit, 1f, _dampTime, Time.fixedDeltaTime);
    }

    public void Exit(in PlayerContext context)
    {
        context.animator.SetFloat(_hashHit, 0.89f);
        context.animator.SetFloat(_hashHit, 0f, _dampTime, Time.fixedDeltaTime);
    }
    #endregion
}
