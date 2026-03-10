#pragma warning disable UNT0010, IDE0090
using System.Text;
using UnityEngine;

/// <summary>
/// 플레이어 상태 관리자
/// </summary>
[System.Serializable]
public class PlayerStateMachine
{
    #region ─────────────────────────▶ 인스펙터 ◀─────────────────────────
    [SerializeField] private PlayerDropImpactState _dropImpact = new PlayerDropImpactState();
    [SerializeField] private PlayerFallState _fall = new PlayerFallState();
    [SerializeField] private PlayerGuardState _guard = new PlayerGuardState();
    [SerializeField] private PlayerHitState _hit = new PlayerHitState();
    [SerializeField] private PlayerAttackState _attack = new PlayerAttackState();
    [SerializeField] private PlayerJumpState _jump = new PlayerJumpState();
    [SerializeField] private PlayerRunState _run = new PlayerRunState();
    [SerializeField] private PlayerWalkState _walk = new PlayerWalkState();
    [SerializeField] private PlayerIdleState _idle = new PlayerIdleState();
    [SerializeField] private bool _log = false;
    #endregion

    private float _stateChangeLockTime = 0f;
    private readonly StringBuilder _sb = new StringBuilder();

    #region ─────────────────────────▶ 공개 멤버 ◀─────────────────────────
    public IPlayerState CurState { get; private set; }

    public void UpdateState(in PlayerContext context)
    {
        if(CurState == null)
        {
            CurState = _idle;
        }
        // 다음 상태 결정 시도
        IPlayerState next = null;
        if (_stateChangeLockTime < Time.fixedTime)
        {
            next = BuildNextState(in context);
        }
        // 상태 변경 시도
        if (next != CurState && next != null)
        {
            ChangeState(next, in context);
        }
        // 상태에 따른 행동 실행
        CurState.Frame(in context);
    }

    // 디버그
    public void DrawText(in PlayerContext context)
    {
        float time = _stateChangeLockTime > Time.fixedTime ? (Time.fixedTime - _stateChangeLockTime) : 0f;
        _sb.Clear();
        _sb.Append($"\nState     = {CurState}");
        _sb.Append($"\nStateLock = {time:F1}초");
        _sb.Append($"\nMove      = {context.inputMove}");
        _sb.Append($"\nRun       = {context.inputRun}");
        _sb.Append($"\nJump      = {context.inputJump}");
        _sb.Append($"\nAttack    = {context.inputAttack}");
        _sb.Append($"\nGuard     = {context.inputGuard}");
        _sb.AppendLine();
        _sb.Append($"\nIsGrounded    = {context.isGrounded}");
        _sb.Append($"\nIsFalling = {context.isFalling}");
        _sb.Append($"\nIsDropImpact     = {context.isDropImpact}");
        _sb.Append($"\nIsHit         = {context.isHit}");
        De.DrawText(_sb.ToString(), 30, De.EWhere.LeftDown);
    }
    #endregion

    #region ─────────────────────────▶ 내부 메서드 ◀─────────────────────────
    private void ChangeState(IPlayerState next, in PlayerContext context)
    {
        De.Log(_log, $"플레이어 상태를 {CurState}에서 {next}로 변경합니다.");
        CurState.Exit(in context);
        CurState = next;
        CurState.Enter(in context, ref _stateChangeLockTime);
    }

    private IPlayerState BuildNextState(in PlayerContext context)
    {
        // 1 : 낙하 충격
        if (context.isDropImpact)
        {
            return _dropImpact;
        }
        // 2 : 낙하
        if (context.isFalling)
        {
            return _fall;
        }
        // 공격 중이라면 아래 상태전환 모두 무시
        if (_attack.IsAttacking)
        {
            return _attack;
        }
        // 3 : 가드
        if (context.inputGuard && context.isGrounded)
        {
            return _guard;
        }
        // 4 : 피격
        if (context.isHit)
        {
            return _hit;
        }
        // 5 : 공격
        if (context.inputAttack && context.isGrounded)
        {
            return _attack;
        }
        // 6 : 점프
        if (context.inputJump || !context.isGrounded)
        {
            return _jump;
        }
        if (context.inputMove != Vector2.zero)
        {
            // 7 : 달리기
            if (context.inputRun)
            {
                return _run;
            }
            // 8 : 걷기
            else
            {
                return _walk;
            }
        }
        // 해당하는 상태가 없다면 Idle
        return _idle;
    }
    #endregion
}
