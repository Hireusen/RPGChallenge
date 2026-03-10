using System.Collections;
using UnityEngine;

/// <summary>
/// 이것이 부착된 오브젝트는 피격, 회복, 삭제당할 수 있습니다.
/// </summary>
public class EntityHeart : MonoBehaviour, IDamageable, IHealable
{
    #region ─────────────────────────▶ 인스펙터 ◀─────────────────────────
    [Header("체력 설정")]
    [SerializeField] private float _maxHealth = 100;
    [SerializeField] private float _curHealth = 100;

    [Header("삭제 연출 시간")]
    [SerializeField] private float _deathDuration = 1f;

    [Header("디버그")]
    [SerializeField] private bool _log = true;
    #endregion

    private bool _isHit = false;
    private bool _isDead = false;

    #region ─────────────────────────▶ 내부 메서드 ◀─────────────────────────
    private IEnumerator CoDestroy()
    {
        // 싹다 가져와서 빨강색으로 칠하기
        Renderer[] renderers = transform.GetComponentsInChildren<Renderer>();
        if(renderers != null)
        {
            int length = renderers.Length;
            for (int i = 0; i < length; ++i)
            {
                Renderer renderer = renderers[i];
                if (renderer == null) continue;
                Material mat = renderer.material;
                if (mat == null) continue;
                Color color = mat.color;
                color.r = Mathf.Min(color.r * 1.5f, 255f);
                color.g = Mathf.Max(color.g * 0.5f, 0f);
                color.b = Mathf.Max(color.b * 0.5f, 0f);
                mat.color = color;
            }
        }
        // 대기 및 삭제
        yield return new WaitForSeconds(_deathDuration);
        Destroy(gameObject);
    }
    #endregion

    #region ─────────────────────────▶ 공개 멤버 ◀─────────────────────────
    public (float curHP, float maxHP) GetHealth() => (_curHealth, _maxHealth);

    public void TakeDamage(float amount)
    {
        if (_isDead)
        {
            return;
        }
        _isHit = true;
        _curHealth -= amount;
        De.Log(_log, $"피격! {amount} 대미지를 받았습니다. (남은 체력 : {_curHealth})");
        if (_curHealth <= 0f)
        {
            De.Log(_log, $"체력이 0 이하가 되었으므로 {gameObject}를 파괴합니다.");
            StartCoroutine(CoDestroy());
        }
    }

    public void Healing(float amount)
    {
        if (_isDead)
        {
            return;
        }
        _curHealth = Mathf.Min(_curHealth + amount, _maxHealth);
        De.Log(_log, $"회복! {amount} 회복을 받았습니다. (현재 체력 : {_curHealth})");
    }

    public bool IsHit()
    {
        if (_isDead)
        {
            return false;
        }
        if (_isHit)
        {
            _isHit = false;
            return true;
        }
        return false;
    }
    #endregion
}
