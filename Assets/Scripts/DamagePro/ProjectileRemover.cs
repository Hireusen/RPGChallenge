using System.Collections.Generic;
using UnityEngine;

public class ProjectileRemover : MonoBehaviour
{
    #region ─────────────────────────▶ 인스펙터 ◀─────────────────────────
    [Header("필터링 설정")]
    [SerializeField] private string _teamTag = "Player";
    [SerializeField] private string _enemyTag = "Enemy";
    [SerializeField] private LayerMask _targetLayer;

    [Header("디버깅")]
    [SerializeField] private bool _log = true;
    #endregion

    #region ─────────────────────────▶ 메시지 함수 ◀─────────────────────────
    private void Awake()
    {
        if (string.IsNullOrEmpty(_teamTag) || string.IsNullOrEmpty(_enemyTag))
        {
            De.Print($"{this.name}에 태그가 존재하지 않습니다. ({_teamTag}, {_enemyTag})", LogType.Assert);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 레이어 검사
        if (!UTool.IsInLayer(other.gameObject, _targetLayer))
        {
            return;
        }
        // 태그 검사
        if (string.IsNullOrEmpty(other.tag) || other.tag == _teamTag || other.tag != _enemyTag)
        {
            return;
        }
        // 삭제
        Destroy(other.gameObject);
        De.Log(_log, "발사체 삭제!");
    }
    #endregion
}
