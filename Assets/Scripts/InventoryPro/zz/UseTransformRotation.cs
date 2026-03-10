using UnityEngine;

/// <summary>
/// 오브젝트에 부착하는 C# 스크립트입니다.
/// 부착된 오브젝트는 특정 회전값으로 n번 회전합니다.
/// </summary>
public class UseTransformRotation : MonoBehaviour
{
    #region ─────────────────────────▶ 내부 변수 ◀─────────────────────────
    private const int ROTATE_COUNT = 30;
    private const float ANGLE_AMOUNT = 180f;
    private int _rotateCount;
    private Quaternion _rotation;
    #endregion

    #region ─────────────────────────▶ 메시지 함수 ◀─────────────────────────
    private Quaternion GetRotation()
    {
        float x = (Random.value * 2f - 1f) * ANGLE_AMOUNT * Time.deltaTime;
        float y = (Random.value * 2f - 1f) * ANGLE_AMOUNT * Time.deltaTime;
        float z = (Random.value * 2f - 1f) * ANGLE_AMOUNT * Time.deltaTime;
        return Quaternion.Euler(x, y, z);
    }

    private void Start()
    {
        _rotateCount = ROTATE_COUNT;
        _rotation = GetRotation();
    }

    private void Update()
    {
        if(_rotateCount <= 0) {
            _rotateCount = ROTATE_COUNT;
            _rotation = GetRotation();
        }
        _rotateCount--;
        transform.rotation *= _rotation;
    }
    #endregion
}
