using System.Collections.Generic;
using UnityEngine;

public class ApplyDamageWithTrigger : MonoBehaviour
{
    #region ─────────────────────────▶ 인스펙터 ◀─────────────────────────
    [Header("전투 설정")]
    [SerializeField] private float _damageAmount = 10f;
    [SerializeField] private bool _onlyOnce = true;

    [Header("필터링 설정")]
    [SerializeField] private string _teamTag = "Player";
    [SerializeField] private string _enemyTag = "Enemy";

    [Header("디버깅")]
    [SerializeField] private bool _log = true;
    #endregion

    private HashSet<Collider> _hitTargets = new HashSet<Collider>();

    #region ─────────────────────────▶ 공개 멤버 ◀─────────────────────────

    public void EnableAttack()
    {
        _hitTargets.Clear();
        GetComponent<Collider>().enabled = true;
        De.Log(_log, $"{gameObject}의 무기 콜라이더를 활성화했습니다.");
    }

    public void DisableAttack()
    {
        Collider col = GetComponent<Collider>();
        if (col.enabled)
        {
            col.enabled = false;
            De.Log(_log, $"{gameObject}의 무기 콜라이더를 비활성화했습니다.");
        }
    }
        #endregion

        #region ─────────────────────────▶ 메시지 함수 ◀─────────────────────────
    private void Awake()
    {
        if(string.IsNullOrEmpty(_teamTag) || string.IsNullOrEmpty(_enemyTag))
        {
            DisableAttack();
            De.Print($"{this.name}에 태그가 존재하지 않습니다. ({_teamTag}, {_enemyTag})", LogType.Assert);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 태그 검사
        if (string.IsNullOrEmpty(other.tag) || other.tag == _teamTag || other.tag != _enemyTag)
        {
            return;
        }
        // 중복 타격 방지
        if (_onlyOnce && _hitTargets.Contains(other))
        {
            return;
        }
        // 맞자
        IDamageable target = other.GetComponent<IDamageable>();
        if (target != null)
        {
            target.TakeDamage(_damageAmount);
            _hitTargets.Add(other);
            De.Log(_log, $"{other.name}에게 {_damageAmount}의 피해를 가했습니다.");
        }
    }
    #endregion
}
