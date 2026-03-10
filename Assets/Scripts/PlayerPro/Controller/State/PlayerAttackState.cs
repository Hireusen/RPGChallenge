using UnityEngine;
// 공격 그래픽만 담당

[System.Serializable]
public class PlayerAttackState : IPlayerState
{
    [Header("참조 연결")]
    [SerializeField] private ApplyDamageWithTrigger _applyDamage;

    [Header("파라미터")]
    [SerializeField] private string _attack1Param = "fAttack1";
    [SerializeField] private string _attack2Param = "fAttack2";
    [SerializeField] private string _attack3Param = "fAttack3";
    [SerializeField, Range(0f, 1f)] private float _dampTime = 0.4f;
    [SerializeField] private float[] _attackDuration = { 0.7f, 0.9f, 1.1f };
    [SerializeField] private float _comboWait = 0.5f;

    [Header("디버그")]
    [SerializeField] private bool _log = false;

    private const int COMBO_COUNT = 3;
    private int[] _hashAttack;
    private bool[] _hasAttackParam;
    private int _combo = 0;
    private float _nextAttackTime = 0f;

    #region ─────────────────────────▶ 공개 멤버 ◀─────────────────────────
    public bool IsAttacking { get; private set; }

    public PlayerAttackState()
    {
        De.Log(_log, "플레이어 DropImpact 생성자가 호출되었습니다.");
        _hashAttack = new int[COMBO_COUNT];
        _hasAttackParam = new bool[COMBO_COUNT];
        // 1
        _hasAttackParam[0] = !string.IsNullOrEmpty(_attack1Param);
        if (_hasAttackParam[0])
        {
            _hashAttack[0] = Animator.StringToHash(_attack1Param);
            De.Log(_log, "성공적으로 Attack 1을 생성했습니다.");
        }
        // 2
        _hasAttackParam[1] = !string.IsNullOrEmpty(_attack2Param);
        if (_hasAttackParam[1])
        {
            _hashAttack[1] = Animator.StringToHash(_attack2Param);
            De.Log(_log, "성공적으로 Attack 2를 생성했습니다.");
        }
        // 3
        _hasAttackParam[2] = !string.IsNullOrEmpty(_attack3Param);
        if (_hasAttackParam[2])
        {
            _hashAttack[2] = Animator.StringToHash(_attack3Param);
            De.Log(_log, "성공적으로 Attack 3을 생성했습니다.");
        }
    }

    public void Enter(in PlayerContext context, ref float stateChangeLockTime)
    {
        _nextAttackTime = 0f;
        context.rb.velocity = Vector3.zero;
        IsAttacking = true;
        _applyDamage.EnableAttack();
    }

    public void Frame(in PlayerContext context)
    {
        De.Log(_log, $"Combo = {_combo}");
        De.Log(_log, $"IsAttack = {IsAttacking}");

        // 콤보 초기화
        if (Time.fixedTime > _nextAttackTime + _comboWait)
        {
            _combo = 0;
            for (int i = 0; i < COMBO_COUNT; ++i)
            {
                context.animator.SetFloat(_hashAttack[i], 0.89f);
                context.animator.SetFloat(_hashAttack[i], 0f, _dampTime, Time.fixedDeltaTime);
            }
        }
        // 지속시간 검사
        if (Time.fixedTime > _nextAttackTime)
        {
            // 콤보 전환
            if (context.inputAttack)
            {
                IsAttacking = true;
                _applyDamage.EnableAttack();
                _combo = _combo < COMBO_COUNT - 1 ? _combo + 1 : 0;
                _nextAttackTime = Time.fixedTime + _attackDuration[_combo];

                // 공격 애니메이션
                for (int i = 0; i < COMBO_COUNT; ++i)
                {
                    if (_combo == i)
                    {
                        context.animator.SetFloat(_hashAttack[i], 0.9f);
                        context.animator.SetFloat(_hashAttack[i], 1f, _dampTime, Time.fixedDeltaTime);
                    }
                    else
                    {
                        context.animator.SetFloat(_hashAttack[i], 0.89f);
                        context.animator.SetFloat(_hashAttack[i], 0f, _dampTime, Time.fixedDeltaTime);
                    }
                }
            }
            // 입력 없음
            else
            {
                IsAttacking = false;
            }
        }
    }

    public void Exit(in PlayerContext context)
    {
        IsAttacking = false;
        for (int i = 0; i < COMBO_COUNT; ++i)
        {
            context.animator.SetFloat(_hashAttack[i], 0.89f);
            context.animator.SetFloat(_hashAttack[i], 0f, _dampTime, Time.fixedDeltaTime);
        }
        _applyDamage.DisableAttack();
        De.Log(_log, $"IsAttack = {IsAttacking}");
    }
    #endregion
}
