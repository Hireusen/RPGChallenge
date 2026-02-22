using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// 쿨타임을 관리하는 유틸리티 클래스입니다.
/// </summary>
public class UCool : MonoBehaviour
{
    /// <summary>
    /// nextTime보다 현재 시간이 크면 True를 반환합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsReady(float nextTime)
    {
        return nextTime < Time.time;
    }

    /// <summary>
    /// nextTime보다 현재 시간이 크면 True를 반환합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsReadyUnscaled(float nextTime)
    {
        return nextTime < Time.unscaledTime;
    }

    /// <summary>
    /// 쿨다운이 끝났다면 nextTime을 갱신하고 True를 반환합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryUpdate(ref float nextTime, float interval)
    {
        // 쿨타임 갱신
        if (IsReady(nextTime)) {
            nextTime = Time.time + interval;
            return true;
        }
        // 쿨타임 불충족
        return false;
    }

    /// <summary>
    /// 쿨다운이 끝났다면 nextTime을 갱신하고 True를 반환합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryUpdateUnscaled(ref float nextTime, float interval)
    {
        // 쿨타임 갱신
        if (IsReadyUnscaled(nextTime)) {
            nextTime = Time.unscaledTime + interval;
            return true;
        }
        // 쿨타임 불충족
        return false;
    }
}
