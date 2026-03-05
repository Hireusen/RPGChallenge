using UnityEngine;

/// <summary>
/// 카메라 오브젝트에 부착하는 C# 스크립트입니다.
/// 3인칭으로 타겟을 따라다닙니다.
/// </summary>
public class CameraTracking : MonoBehaviour
{
    #region ─────────────────────────▶ 인스펙터 ◀─────────────────────────
    [Header("카메라 등록")]
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _targetTr;

    [Header("카메라 설정")]
    [SerializeField] private Vector3 _offset = new Vector3(0f, 2f, -2f);
    [SerializeField] private float _lookHeight = 1.5f;
    [SerializeField] private float _sharpness = 8f;

    [Header("디버그")]
    [SerializeField] private bool _log = false;
    [SerializeField] private bool _ray = false;
    #endregion

    private Transform _cameraTr;

    #region ─────────────────────────▶ 내부 메서드 ◀─────────────────────────
    // 해당 위치로 즉시 이동
    private void InitPose()
    {
        BuildPose(out Vector3 desiredPos, out Quaternion desiredRot);
        ApplyPose(desiredPos, desiredRot, _sharpness, true);
    }
    // 매 프레임 따라붙기
    private void TickMove()
    {
        BuildPose(out Vector3 desiredPos, out Quaternion desiredRot);
        ApplyPose(desiredPos, desiredRot, _sharpness, false);
    }
    // 카메라가 이동할 좌표와 회전 계산
    private void BuildPose(out Vector3 desiredPos, out Quaternion desiredRot)
    {
        desiredPos = _targetTr.position + (_targetTr.rotation * _offset);
        Vector3 lookPos = _targetTr.position + Vector3.up * _lookHeight;
        desiredRot = Quaternion.LookRotation(lookPos - desiredPos, Vector3.up);
    }
    // 스냅 또는 보간하여 실제 적용
    private void ApplyPose(Vector3 desiredPos, Quaternion desiredRot, float sharpness, bool snap = false)
    {
        if (snap) {
            _cameraTr.position = desiredPos;
            _cameraTr.rotation = desiredRot;
        }
        float t = UMath.GetSmoothT(sharpness, Time.deltaTime);
        _cameraTr.position = Vector3.Lerp(_cameraTr.position, desiredPos, t);
        _cameraTr.rotation = Quaternion.Slerp(_cameraTr.rotation, desiredRot, t);
    }
    #endregion

    #region ─────────────────────────▶ 메시지 함수 ◀─────────────────────────
    private void Awake()
    {
        if (De.IsNull(_camera)) {
            _camera = FindFirstObjectByType<Camera>();
            if(_camera == null) {
                enabled = false;
                De.Print("카메라가 어디갔나요?", LogType.Assert);
            }
        }
        _cameraTr = _camera.transform;
    }

    private void LateUpdate()
    {
        if (_targetTr == null || _camera == null) {
            return;
        }
        TickMove();
        if (_ray) {
            Debug.DrawLine(_cameraTr.position, _targetTr.position, Color.magenta);
        }
    }
    #endregion
}
