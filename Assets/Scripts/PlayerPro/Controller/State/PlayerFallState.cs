using UnityEngine;

[System.Serializable]
public class PlayerFallState : IPlayerState
{
    [Header("파라미터")]
    [SerializeField] private string _fallParam = "fFall";
    [SerializeField, Range(0f, 1f)] private float _dampTime = 0.12f;

    [Header("이동 설정")]
    [SerializeField] private float _walkSpeed = 5.4f;
    [SerializeField] private float _runSpeed = 8.1f;
    [SerializeField] private float _rotateSharpness = 1.5f;

    [Header("디버그")]
    [SerializeField] private bool _log = false;

    private int _hashFall;
    private bool _hasFallParam;

    #region ─────────────────────────▶ 공개 멤버 ◀─────────────────────────
    public PlayerFallState()
    {
        De.Log(_log, "플레이어 Fall 생성자가 호출되었습니다.");
        _hasFallParam = !string.IsNullOrEmpty(_fallParam);
        if (_hasFallParam)
        {
            _hashFall = Animator.StringToHash(_fallParam);
            De.Log(_log, "성공적으로 Fall Hash를 생성했습니다.");
        }
    }

    public void Enter(in PlayerContext context, ref float stateChangeLockTime)
    {
        context.animator.SetFloat(_hashFall, 0.9f);
        context.animator.SetFloat(_hashFall, 1f, _dampTime, Time.fixedDeltaTime);
    }

    public void Frame(in PlayerContext context)
    {
        context.animator.SetFloat(_hashFall, 1f, _dampTime, Time.fixedDeltaTime);

        // 공중 수평 이동 로직
        Vector3 dir = UPlayerState.BuildMoveDirection(in context);
        if (dir != Vector3.zero)
        {
            float speed = context.inputRun ? _runSpeed : _walkSpeed;
            UPlayerState.SetGroundVelocity(in context, dir, speed);
            UPlayerState.Rotate(in context, dir, _rotateSharpness);
        }
    }

    public void Exit(in PlayerContext context)
    {
        context.animator.SetFloat(_hashFall, 0.89f);
        context.animator.SetFloat(_hashFall, 0f, _dampTime, Time.fixedDeltaTime);
    }
    #endregion
}
