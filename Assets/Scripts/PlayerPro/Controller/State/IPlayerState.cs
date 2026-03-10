/// <summary>
/// 모든 플레이어 상태가 구현해야 하는 인터페이스.
/// State는 전이 조건을 일절 알지 못하며, 자신의 행동만 수행합니다.
/// </summary>
public interface IPlayerState
{
    void Enter(in PlayerContext context, ref float stateChangeLockTime);
    void Frame(in PlayerContext context);
    void Exit(in PlayerContext context);
}
