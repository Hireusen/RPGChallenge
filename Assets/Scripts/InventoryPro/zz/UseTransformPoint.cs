using UnityEngine;

/// <summary>
/// 오브젝트에 부착하는 C# 스크립트입니다.
/// 부착된 오브젝트의 정면에 랜덤으로 구가 생성됩니다.
/// </summary>
public class UseTransformPoint : MonoBehaviour
{
    #region ─────────────────────────▶ 내부 변수 ◀─────────────────────────
    private const int SPHERE_COUNT = 2;
    private const float SPHERE_RADIUS = 0.1f;
    private const float SPHERE_SPREAD = 2f;
    private Color _color;
    private Vector3[] _pos;
    #endregion

    #region ─────────────────────────▶ 메시지 함수 ◀─────────────────────────
    private void Start()
    {
        _color = URand.GetColor();
        _pos = new Vector3[SPHERE_COUNT];
    }

    private void Update()
    {
        for (int i = 0; i < SPHERE_COUNT; ++i) {
            Vector3 offset = (Vector3.forward + Random.insideUnitSphere) * SPHERE_SPREAD;
            _pos[i] = transform.TransformPoint(offset);
        }
    }

    private void OnDrawGizmos()
    {
        if (_pos == null)
            return;
        for (int i = 0; i < _pos.Length; ++i) {
            Gizmos.color = _color;
            Gizmos.DrawSphere(_pos[i], SPHERE_RADIUS);
        }
    }
    #endregion
}
