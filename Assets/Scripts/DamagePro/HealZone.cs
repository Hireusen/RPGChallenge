using UnityEngine;

/// <summary>
/// 클래스의 설계 의도입니다.
/// </summary>
public class HealZone : MonoBehaviour
{
    #region ─────────────────────────▶ 인스펙터 ◀─────────────────────────
    [Header("힐 이펙트 프리펩")]
    [SerializeField] private GameObject _healPrefab;

    [Header("회복 설정")]
    [SerializeField] private float _healInterval = 0.1f;
    [SerializeField] private float _healAmount = 1f;

    [Header("디버그")]
    [SerializeField] private bool _log = false;
    #endregion

    #region ─────────────────────────▶ 내부 변수 ◀─────────────────────────
    private float _nextHealTime;
    private bool _nowHeal = false;
    #endregion

    #region ─────────────────────────▶ 메시지 함수 ◀─────────────────────────
    private void OnTriggerStay(Collider other)
    {
        if (_nowHeal)
        {
            if (other.TryGetComponent<IHealable>(out IHealable target))
            {
                target.Healing(_healAmount);
                // 이펙트
                Vector3 pos = other.transform.position;
                pos.y += 1f;
                GameObject go = Instantiate(_healPrefab, pos, Quaternion.identity);
                Destroy(go, _healInterval);
            }
        }
    }

    private void Awake()
    {
        _nextHealTime = Time.fixedTime;
    }

    private void FixedUpdate()
    {
        if (Time.fixedTime > _nextHealTime)
        {
            _nextHealTime += _healInterval;
            _nowHeal = true;
        }
        else
        {
            _nowHeal = false;
        }
    }
    #endregion
}
