using UnityEngine;

/// <summary>
/// 오브젝트에 부착하는 C# 스크립트입니다.
/// 부착된 오브젝트는 랜덤 Transform을 바라보며 오른쪽으로 이동합니다.
/// </summary>
public class UseTransformLookAt : MonoBehaviour
{
    #region ─────────────────────────▶ 내부 변수 ◀─────────────────────────
    [SerializeField] private Transform _target;
    private const float MOVE_SPEED = 25.555f;
    #endregion

    #region ─────────────────────────▶ 메시지 함수 ◀─────────────────────────
    private void TrySetTarget()
    {
        if (_target == null) {
            _target = URand.GetTransform();
        }
    }

    private void Update()
    {
        TrySetTarget();
        transform.LookAt(_target);
        transform.Translate(Vector3.right * MOVE_SPEED * Time.deltaTime);
    }
    #endregion
}
