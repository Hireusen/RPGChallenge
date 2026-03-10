using UnityEngine;

/// <summary>
/// 오브젝트에 부착하는 C# 스크립트입니다.
/// 부착된 오브젝트는 주변 랜덤 좌표를 기준으로 회전합니다.
/// </summary>
public class UseTransformRotateAround : MonoBehaviour
{
    #region ─────────────────────────▶ 내부 변수 ◀─────────────────────────
    [SerializeField] private Vector3 _centerPoint;
    [SerializeField] private Vector3 _originAxis;
    private const float ROTATION_SPEED = 135f;
    private const float CENTER_DISTANCE = 5.5f;
    #endregion

    #region ─────────────────────────▶ 메시지 함수 ◀─────────────────────────
    private void Start()
    {
        // 랜덤 축
        _originAxis = URand.GetAxis();
        // 랜덤 좌표
        Vector3 dir = Random.onUnitSphere;
        _centerPoint = transform.position + dir * CENTER_DISTANCE;
    }

    private void Update()
    {
        transform.RotateAround(_centerPoint, _originAxis, ROTATION_SPEED * Time.deltaTime);
    }
    #endregion
}
