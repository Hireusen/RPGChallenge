using UnityEngine;

/// <summary>
/// 오브젝트에 부착하는 C# 스크립트입니다.
/// 부착된 오브젝트는 랜덤 축으로 회전합니다.
/// </summary>
public class UseTransformRotateLocal : MonoBehaviour
{
    #region ─────────────────────────▶ 내부 변수 ◀─────────────────────────
    [SerializeField] private Vector3 _originAxis;
    private const float ROTATION_SPEED = 135f;
    #endregion

    #region ─────────────────────────▶ 메시지 함수 ◀─────────────────────────
    private void Start()
    {
        _originAxis = URand.GetAxis();
    }

    private void Update()
    {
        transform.Rotate(_originAxis, ROTATION_SPEED * Time.deltaTime, Space.Self);
    }
    #endregion
}
