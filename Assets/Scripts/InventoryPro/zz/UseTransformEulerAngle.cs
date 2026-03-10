using UnityEngine;

/// <summary>
/// 오브젝트에 부착하는 C# 스크립트입니다.
/// 부착된 오브젝트는 Y축을 기준으로 빠르게 회전합니다.
/// </summary>
public class UseTransformEulerAngle : MonoBehaviour
{
    #region ─────────────────────────▶ 내부 변수 ◀─────────────────────────
    [SerializeField] private Vector3 _originAxis;
    private const float ROTATION_SPEED = 270f;
    #endregion

    #region ─────────────────────────▶ 메시지 함수 ◀─────────────────────────
    private void Update()
    {
        float degreeY = transform.eulerAngles.y;
        degreeY += ROTATION_SPEED * Time.deltaTime;
        transform.eulerAngles = new Vector3(0f, degreeY, 0f);
    }
    #endregion
}
