using UnityEngine;

/// <summary>
/// PlayerState 전용 유틸리티
/// </summary>
public static class UPlayerState
{
    /// <summary>
    /// 이동 방향을 반환합니다.
    /// </summary>
    public static Vector3 BuildMoveDirection(in PlayerContext context)
    {
        Vector3 input = new Vector3(context.inputMove.x, 0f, context.inputMove.y); 
        Vector3 dir = Vector3.ClampMagnitude(input, 1f);
        if (input.sqrMagnitude < 0.0001f)
        {
            return Vector3.zero;
        }
        Vector3 forward = Vector3.ProjectOnPlane(context.cameraTr.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(context.cameraTr.right, Vector3.up).normalized;
        return (forward * dir.z + right * dir.x).normalized;
    }

    /// <summary>
    /// 이동 속도를 설정합니다.
    /// </summary>
    public static void SetGroundVelocity(in PlayerContext context, Vector3 dir, float speed)
    {
        Vector3 vel = speed * dir;
        vel.y = context.rb.velocity.y;
        context.rb.velocity = vel;
    }

    /// <summary>
    /// 이동을 멈춥니다.
    /// </summary>
    public static void StopGroundVelocity(in PlayerContext context)
    {
        context.rb.velocity = new Vector3(0f, context.rb.velocity.y, 0f);
    }

    /// <summary>
    /// 해당 방향으로 회전합니다.
    /// </summary>
    public static void Rotate(in PlayerContext context, Vector3 dir, float sharpness)
    {
        if (dir == Vector3.zero)
        {
            return;
        }
        // 지수 보간
        Quaternion desired = Quaternion.LookRotation(dir, Vector3.up);
        float t = UMath.GetSmoothT(sharpness, Time.fixedDeltaTime);
        context.rb.MoveRotation(Quaternion.Slerp(context.rb.rotation, desired, t));
    }
}
