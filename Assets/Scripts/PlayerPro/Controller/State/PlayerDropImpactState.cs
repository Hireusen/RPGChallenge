using UnityEngine;

/// <summary>
/// 클래스의 설계 의도입니다.
/// </summary>
[System.Serializable]
public class PlayerDropImpactState : IPlayerState
{
    [Header("파라미터")]
    [SerializeField] private string _dropImpactParam = "fDropImpact";
    [SerializeField, Range(0f, 1f)] private float _dampTime = 0.12f;
    [SerializeField, Range(0f, 2f)] private float _lockTime = 1f;

    [Header("디버그")]
    [SerializeField] private bool _log = false;

    private int _hashDropImpact;
    private bool _hasDropImpactParam;

    #region ─────────────────────────▶ 공개 멤버 ◀─────────────────────────
    public PlayerDropImpactState()
    {
        De.Log(_log, "플레이어 DropImpact 생성자가 호출되었습니다.");
        _hasDropImpactParam = !string.IsNullOrEmpty(_dropImpactParam);
        if (_hasDropImpactParam)
        {
            _hashDropImpact = Animator.StringToHash(_dropImpactParam);
            De.Log(_log, "성공적으로 DropImpact Hash를 생성했습니다.");
        }
    }

    public void Enter(in PlayerContext context, ref float stateChangeLockTime)
    {
        stateChangeLockTime = Time.fixedTime + _lockTime;
        context.rb.velocity = Vector3.zero;
        context.animator.SetFloat(_hashDropImpact, 0.9f);
        context.animator.SetFloat(_hashDropImpact, 1f, _dampTime, Time.fixedDeltaTime);
    }

    public void Frame(in PlayerContext context)
    {
        context.animator.SetFloat(_hashDropImpact, 1f, _dampTime, Time.fixedDeltaTime);
    }

    public void Exit(in PlayerContext context)
    {
        context.animator.SetFloat(_hashDropImpact, 0.89f);
        context.animator.SetFloat(_hashDropImpact, 0f, _dampTime, Time.fixedDeltaTime);
    }
    #endregion
}
