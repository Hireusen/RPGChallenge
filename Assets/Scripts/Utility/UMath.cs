using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// 수학 연산을 돕는 유틸리티 클래스입니다.
/// </summary>
public static class UMath
{
    /// <summary>
    /// 원하는 높이에 도달하기 위한 초기 점프 속도를 계산합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float CalcJumpVelocity(float targetHeight, float gravity)
    {
        return Mathf.Sqrt(targetHeight * -2f * gravity);
    }
}
