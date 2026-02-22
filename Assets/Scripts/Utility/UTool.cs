using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// 전반적으로 자주 사용되는 유틸리티 클래스입니다.
/// </summary>
public static class UTool
{
    #region 컴포넌트 & 게임 오브젝트
    /// <summary>
    /// 게임 오브젝트의 컴포넌트를 반환합니다.
    /// 없을 경우 새로 생성합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetOrAddComponent<T>(GameObject go) where T : Component
    {
        if (go == null) {
            De.Print("게임 오브젝트가 존재하지 않습니다.", LogType.Warning);
            return null;
        }
        // 실행 코드
        T component = go.GetComponent<T>();
        if (component == null) {
            component = go.AddComponent<T>();
        }
        return component;
    }

    /// <summary>
    /// 부모 Transform의 모든 자식을 파괴합니다.
    /// </summary>
    public static void DestroyAllChildren(Transform parent)
    {
        if (parent == null) {
            De.Print("트랜스폼이 존재하지 않습니다.", LogType.Warning);
            return;
        }
        // 역반복하며 파괴
        int length = parent.childCount - 1;
        for (int i = length; i >= 0; --i) {
            UnityEngine.Object.Destroy(parent.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// 게임오브젝트의 활성 상태를 매개 변수로 설정합니다.
    /// 활성 상태가 변경될 경우 true를 반환합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TrySetActive(GameObject go, bool active)
    {
        if (go == null) {
            De.Print("게임 오브젝트가 존재하지 않습니다.", LogType.Warning);
            return false;
        }
        // 실행 코드
        if (go.activeSelf != active) {
            go.SetActive(active);
            return true;
        }
        return false;
    }
    #endregion

    #region 레이어 & 태그
    /// <summary>
    /// 레이어 이름으로 LayerMask 비트를 생성합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LayerToBit(string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        if(layer >= 0) {
            return 1 << layer;
        }
        De.Print($"존재하지 않는 레이어 이름 {layerName}을 받았습니다.", LogType.Assert);
        return 0;
    }

    /// <summary>
    /// 여러 레이어 이름으로 복합 LayerMask 비트를 생성합니다.
    /// </summary>
    public static int LayersToBit(params string[] layerNames)
    {
        if (layerNames == null) {
            De.Print($"레이어를 전달받지 못했습니다.", LogType.Assert);
            return 0;
        }
        // 실행 코드
        int mask = 0;
        int length = layerNames.Length;
        for (int i = 0; i < length; ++i) {
            int layer = LayerMask.NameToLayer(layerNames[i]);
            if (layer >= 0) {
                mask |= 1 << layer;
            } else {
                De.Print($"존재하지 않는 레이어 이름 {layerNames[i]}을 받았습니다.", LogType.Assert);
            }
        }
        return mask;
    }

    /// <summary>
    /// 대상 게임 오브젝트가 지정한 레이어에 속하는지 검사합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInLayer(GameObject go, int layerMask)
    {
        if (go == null) {
            De.Print("게임 오브젝트가 존재하지 않습니다.", LogType.Warning);
            return false;
        }
        // 실행 코드
        return (layerMask & (1 << go.layer)) != 0;
    }
    #endregion
  
    /// <summary>
    /// HEX 문자열을 Color로 변환합니다.
    /// 실패하면 Color.white를 반환합니다.
    /// </summary>
    public static Color HexToColor(string hex)
    {
        // 방어 코드
        if (string.IsNullOrEmpty(hex)) {
            De.Print("전달받은 Hex 코드가 비어있습니다.", LogType.Assert);
            return Color.white;
        }
        if (hex[0] == '#') {
            De.Print("Hex 코드에 #이 누락되었습니다.", LogType.Assert);
            hex = hex.Substring(1);
        }
        if (hex.Length < 6) {
            De.Print($"Hex 코드({hex})의 길이가 6 미만입니다.", LogType.Assert);
            return Color.white;
        }
        // 실행 코드
        if (ColorUtility.TryParseHtmlString("#" + hex, out Color color)) {
            return color;
        } else {
            De.Print($"Hex 코드 변환을 실패했습니다. ({hex})", LogType.Assert);
            return Color.white;
        }
    }

    /// <summary>
    /// 0 ~ 1 범위 확률로 True를 반환합니다.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Chance(float chance)
    {
        return UnityEngine.Random.value < chance;
    }

    #region 애플리케이션
    public static void GameStop()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    #endregion
}
