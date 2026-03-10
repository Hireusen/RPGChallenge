using UnityEngine;

/// <summary>
/// 정면으로 플레이어를 공격하는 총알을 발사합니다.
/// </summary>
public class BulletShooter : MonoBehaviour
{
    #region ─────────────────────────▶ 인스펙터 ◀─────────────────────────
    [Header("총알 설정")]
    [SerializeField] private GameObject _bullet;
    [SerializeField] private float _bulletDuration = 5f;

    [Header("스폰 설정")]
    [SerializeField] private float _spawnInterval = 2f;
    [SerializeField] private float _force = 10f;
    #endregion

    #region ─────────────────────────▶ 내부 변수 ◀─────────────────────────
    private float _nextSpawnTime = 0f;
    #endregion

    #region ─────────────────────────▶ 내부 메서드 ◀─────────────────────────
    private void ShootBullet()
    {
        Quaternion rot = transform.rotation;
        GameObject go = Instantiate(_bullet, transform.position, rot);
        Rigidbody rb = go.GetComponent<Rigidbody>();
        Vector3 dir = rot * Vector3.forward;
        rb.velocity = dir.normalized * _force;
        Destroy(go, _bulletDuration);
    }
    #endregion

    #region ─────────────────────────▶ 메시지 함수 ◀─────────────────────────
    private void Update()
    {
        if(Time.time > _nextSpawnTime)
        {
            _nextSpawnTime = Time.time + _spawnInterval;
            ShootBullet();
        }
    }
    #endregion
}
