using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 클래스의 설계 의도입니다.
/// </summary>
public class HealthBarUI : MonoBehaviour
{
    #region ─────────────────────────▶ 인스펙터 ◀─────────────────────────
    [Header("참조 연결")]
    [SerializeField] private Transform _canvasTr;
    [SerializeField] private Camera _camera;
    [SerializeField] private Image _greenBar;
    [SerializeField] private EntityHeart _heart;
    #endregion

    #region ─────────────────────────▶ 메시지 함수 ◀─────────────────────────
    private void Awake()
    {
        De.IsNull(_canvasTr);
        De.IsNull(_camera);
        De.IsNull(_greenBar);
        De.IsNull(_heart);
    }

    private void Update()
    {
        // UI 갱신
        (float curHP, float maxHP) = _heart.GetHealth();
        if(curHP <= 0 || maxHP <= 0)
        {
            _greenBar.fillAmount = 0f;
        }
        _greenBar.fillAmount = curHP / maxHP;
    }

    // 카메라에 회전방향 맞추기
    private void LateUpdate()
    {
        if (_camera == null)
        {
            return;
        }
        _canvasTr.rotation = _camera.transform.rotation;
    }
    #endregion
}
